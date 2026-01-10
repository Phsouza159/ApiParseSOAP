using Api.Domain.Api;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using System.Reflection;

namespace Api.Domain.Helper
{
    public static class ProcessadoresHelper
    {

        private enum StatusConfiguracao
        {
            PENDENTE = 1,
            SUCESSO = 2,
            ERRO = 3
        }

        private delegate object PROC_VALUE(object valor);

        private static StatusConfiguracao Status { get; set; } = StatusConfiguracao.PENDENTE;

        private static PROC_VALUE[] Processadores { get; set; }

        #region CARREGAR PROCESSADORES

        public static void CarregarProcessadores()
        {
            if (Status == StatusConfiguracao.SUCESSO)
                return;

            try
            {
                int length = System.Enum.GetValues(typeof(Enum.TiposProcessadores)).Length;
                ProcessadoresHelper.Processadores = new PROC_VALUE[length];

                string? pastaAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // RECUPERAR ARQUIVO DE CONFIGURACAO DE PARAMETROS
                string arquivoConfiguracao = Path.Combine(pastaAssembly, "Configuracao", "CONFIG_PARAMETROS_CONVERSAO.ini");

                if (!File.Exists(arquivoConfiguracao))
                    throw new ArgumentException("Arquivo de configuracao de conversão não localizado.");

                string[] linhas = File.ReadAllLines(arquivoConfiguracao);

                foreach (var linha in linhas)
                {
                    if (!linha.Contains("=")) throw new ArgumentException($"Linha inválida: {linha}");

                    string tipoProcessador = linha.Split("=")[0].Trim();
                    string nomeProc = linha.Split("=")[1].Trim();

                    if (System.Enum.TryParse(tipoProcessador, out TiposProcessadores tipo))
                    {

                        MethodInfo metodo = typeof(ConversorValorHelper).GetMethod(
                            nomeProc,
                            bindingAttr: BindingFlags.Static
                                         | BindingFlags.Public
                                         | BindingFlags.NonPublic
                        );

                        PROC_VALUE proc = (PROC_VALUE)Delegate.CreateDelegate(typeof(PROC_VALUE), metodo);
                        ProcessadoresHelper.Processadores[(short)tipo] = proc;
                    }
                }

                Status = StatusConfiguracao.SUCESSO;
            }
            catch (Exception ex)
            {
                Status = StatusConfiguracao.ERRO;
                throw;
            }
        }

        #endregion

        #region RECUPERAR PROCESSADOR DO ELEMENTO

        private static PROC_VALUE RecuperarProcElemento(TiposProcessadores tiposProcessador)
        {
            int index = (short)tiposProcessador;

            if (index > ProcessadoresHelper.Processadores.Length)
                throw new ArgumentException($"Processador não configurado: {tiposProcessador}$");
           
            // DEFAULT
            if (ProcessadoresHelper.Processadores[index] is null)
                return ConversorValorHelper.PRC_DEFAULT;

            return ProcessadoresHelper.Processadores[index];
        }

        public static object? CarregarValorFormatado(this Elemento elemento, object valor)
        {

            ProcessadoresHelper.CarregarProcessadores();

            try
            {
                if (valor is null)
                    return null;

                PROC_VALUE processador = ProcessadoresHelper.RecuperarProcElemento(elemento.Processador.TiposProcessador);

                return processador(valor);
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro elemento: '{elemento.Nome}'. Mensagem: '{ex.Message}'";
                elemento.Notificacoes.AdicionarMensagem(mensagem);

                // VALOR DEFAULT PARA ERRO
                return null;
            }
        }

  

        #endregion
    }
}
