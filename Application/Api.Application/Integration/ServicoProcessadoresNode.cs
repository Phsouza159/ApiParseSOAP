using Api.Domain.Api;
using Api.Domain.Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Extensions;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Integration;
using Api.Domain.ObjectValues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Api.Application.Integration
{
    public class ServicoProcessadoresNode : ObjetoBase, IServicoProcessadoresNode
    {
        private IConfiguration Configuration { get; }

        public ServicoProcessadoresNode(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public async Task Executar(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            string appNode = Configuration.GetAppNode();

            servicoLog.CriarLog(schema.Servico.Nome, $"Iniciando processador: {contrato.Api}", TipoLog.INFO);
            string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(envelope.ConteudoEnvio));

            var processo = new Process();
            processo.StartInfo.FileName = "node";
            processo.StartInfo.Arguments = $"{appNode} {contrato.Api} '{data}'";
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;

            processo.Start();

            string dataProcessamento = await processo.StandardOutput.ReadToEndAsync();
            string dataErro = await processo.StandardError.ReadToEndAsync();

            await processo.WaitForExitAsync();

            this.TratarRetornoProcessador(
                contrato
                , schema
                , envelope
                , servicoLog
                , dataProcessamento
                , dataErro
            );
        }

        private void TratarRetornoProcessador(
               Contrato contrato
             , Schema schema
             , EnvelopeEnvio envelope
             , IServicoLog servicoLog
             , string dataProcessamento
             , string dataErro
            )
        {
            var objetoResultado = JsonConvert.DeserializeObject<ResultadoProcessamentoBatch>(dataProcessamento);

            if (objetoResultado.Sucesso)
            {
                servicoLog.CriarLog(schema.Servico.Nome, objetoResultado.Mensagem, TipoLog.RETORNO_JSON);
                envelope.ConteudoRetorno = objetoResultado.Mensagem;
                return;
            }

            if (!string.IsNullOrEmpty(dataErro))
            {
                servicoLog.CriarLog(schema.Servico.Nome, dataErro, TipoLog.TRACE_ERRO);
                throw new ArgumentException($"Erro registrado no processador configurado '{contrato.Api}'. Saida console: {dataErro}");
            }

            servicoLog.CriarLog(schema.Servico.Nome, objetoResultado.Mensagem, TipoLog.TRACE_ERRO);
            throw new ArgumentException($"Processamento sem sucesso para o processador '{contrato.Api}'. Saida console: {objetoResultado.Mensagem}");
        }

    }
}
