using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Api.Domain.Services
{
    public class ServicoIntegracao : IServicoIntegracao
    {
        public ServicoIntegracao()
        {
        }

        public async Task Enviar(Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            var contrato = schema.GetContrato();

            if (contrato != null)
            {
                switch (contrato.Tipo)
                {
                    case "POST":
                        await this.Post(contrato, schema, envelope, servicoLog);
                        return;

                    case "FILE":
                        await this.File(contrato, schema, envelope, servicoLog);
                        return;

                    default:
                        throw new ArgumentException($"Sem tratamento para envio: {contrato.Tipo}");
                }
            }
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

        #region REQUEST POST

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
