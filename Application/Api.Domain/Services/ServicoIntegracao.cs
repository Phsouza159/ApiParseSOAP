using Api.Domain.Api;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Extensions;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace Api.Domain.Services
{
    public class ServicoIntegracao : IServicoIntegracao
    {
        private IConfiguration Configuration { get; }

        public ServicoIntegracao(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task Enviar(Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            var contrato = schema.GetContrato();

            if (contrato != null && System.Enum.TryParse(contrato.Tipo, out TipoIntegracao tipoIntegracao))
            {
                switch (tipoIntegracao)
                {
                    case TipoIntegracao.POST:
                        await this.Post(contrato, schema, envelope, servicoLog);
                        return;

                    case TipoIntegracao.FILE:
                        await this.File(contrato, schema, envelope, servicoLog);
                        return;

                    case TipoIntegracao.PROCESSADOR_NODE:
                        await this.Processador(contrato, schema, envelope, servicoLog);
                        return;
                }

                throw new ArgumentException($"Tipo de integração sem suporte '{contrato.Tipo}'.");
            }

            throw new ArgumentException($"Sem implementação para integração.");
        }

        #region FILE

        private async Task File(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            // TODO: ADICIONAR AUTENTICACAO

            if (!System.IO.File.Exists(contrato.Api))
                throw new ArgumentException($"Arquivo não localizado em: {contrato.Api}");

            try
            {
                envelope.ConteudoRetorno = await System.IO.File.ReadAllTextAsync(contrato.Api);

                // ITEM DE VALIDACAO JSON
                JToken.Parse(envelope.ConteudoRetorno);
                servicoLog.CriarLog(schema.Servico.Nome, envelope.ConteudoRetorno, TipoLog.RETORNO_JSON);
            }
            catch (JsonReaderException ex)
            {
                throw new ArgumentException("JSON com formatado inválido.", ex);
            }
        }

        #endregion

        #region POST

        internal async Task Post(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            using var client = new HttpClient();

            this.CarregarAutenticacao(client, schema, servicoLog);

            servicoLog.CriarLog(schema.Servico.Nome, $"REQUEST POST PARA: '{contrato.Api}'", TipoLog.INFO);

            var content = new StringContent(envelope.ConteudoEnvio, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(contrato.Api, content);

            servicoLog.CriarLog(schema.Servico.Nome, $"RETORNO POST PARA: '{contrato.Api}' - STATUS: '{response.StatusCode}' ", TipoLog.INFO);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new AutorizacaoException($"Sem autorização para chamada em '{schema.Servico.Nome}'.");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                envelope.ConteudoRetorno = await response.Content.ReadAsStringAsync();
                servicoLog.CriarLog(schema.Servico.Nome, envelope.ConteudoRetorno, TipoLog.RETORNO_JSON);
                return;
            }

            throw new ArgumentException($"Sem tratamento para retorno: {response.StatusCode}");
        }

        #endregion

        #region PROCESSADOR

        internal async Task Processador(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
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


        #endregion

        #region CARREGAR DADOS PARA AUTENTICACAO

        internal void CarregarAutenticacao(HttpClient client, Schema schema, IServicoLog servicoLog)
        {
            if (schema.Autenticacao != null)
            {
                schema.Autenticacao.CarregarAutenticacao(client, servicoLog);
            }
        }


        #endregion
    }
}
