using Api.Domain.Enum;
using Api.Domain.Exceptions;

namespace Api.Domain.Helper
{
    public static class ProcessadoresHelper
    {

        private delegate object PROC_VALUE(object valor);

        private static PROC_VALUE[] Processadores { get; set; }

        private static bool IsProcessadorSucesso = ProcessadoresHelper.Processadores != null && ProcessadoresHelper.Processadores.Length > 0;

        #region CARREGAR PROCESSADORES

        private static void CarregarProcessadores()
        {
            if (ProcessadoresHelper.IsProcessadorSucesso)
                return;

            int length = System.Enum.GetValues(typeof(Enum.TiposProcessadores)).Length;
            ProcessadoresHelper.Processadores = new PROC_VALUE[length];

            #region PROCS 

            ProcessadoresHelper.Processadores[(short)TiposProcessadores.UNSIGNEDLONG]   = ConversorValorHelper.PRC_USINGNEDLONG;
            ProcessadoresHelper.Processadores[(short)TiposProcessadores.SHORT]          = ConversorValorHelper.PRC_SHORT;
            ProcessadoresHelper.Processadores[(short)TiposProcessadores.STRING]         = ConversorValorHelper.PRC_STRING;
            ProcessadoresHelper.Processadores[(short)TiposProcessadores.INTEGER]        = ConversorValorHelper.PRC_INTEGER;
            ProcessadoresHelper.Processadores[(short)TiposProcessadores.LONG]           = ConversorValorHelper.PRC_LONG;

            #endregion
        }

        #endregion

        public static object? CarregarValorFormatado(this Element elemento, object valor)
        {
            try
            {
                ProcessadoresHelper.CarregarProcessadores();

                PROC_VALUE processador = ProcessadoresHelper.Processadores[(short)elemento.Processador.TiposProcessador];

                if (processador == null)
                    processador = ConversorValorHelper.PRC_DEFAULT;

                if (valor is null)
                    return null;

                return processador(valor);
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro elemento: {elemento.Nome}. Valor: {valor}. Mensagem: '{ex.Message}'";
                elemento.Notificacoes.AdicionarMensagem(mensagem);

                // VALOR DEFAULT PARA ERRO
                return null;
            }
        }
    }
}
