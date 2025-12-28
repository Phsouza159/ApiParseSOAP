using Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using System.Text;
using System.Web.Helpers;

namespace Api.Domain.Services
{
    public class ServicoWeb : IServicoWeb
    {
        public ServicoWeb()
        {
        }

        public async Task Enviar(Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            var contrato = schema.Servico.Contratos.FirstOrDefault(e => e.Servico.ToLower().Equals(schema.NomeServico.ToLower()));

            if (contrato != null)
            {
                switch (contrato.Tipo)
                {
                    case "POST":
                         await this.Post(contrato, schema, envelope, servicoLog);
                        return;

                    default:
                        throw new ArgumentException($"Sem tratamento para envio: {contrato.Tipo}");
                }
            }
        }

        #region REQUEST POST

        internal async Task Post(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            using var client = new HttpClient();

            servicoLog.CriarLog(schema.Servico.Nome, $"REQUEST POST PARA: '{contrato.Api}'", TipoLog.INFO);

            var content = new StringContent(envelope.ConteudoEnvio, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(contrato.Api, content);

            servicoLog.CriarLog(schema.Servico.Nome, $"RETORNO POST PARA: '{contrato.Api}' - STATUS: '{response.StatusCode}' ", TipoLog.INFO);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new AutorizacaoException($"Sem autorização para chamada em '{schema.Servico.Nome}'.");
            }

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                envelope.ConteudoRetorno = await response.Content.ReadAsStringAsync();
                servicoLog.CriarLog(schema.Servico.Nome, envelope.ConteudoRetorno, TipoLog.RETORNO_JSON);
                return;
            }

            throw new ArgumentException($"Sem tratamento para retorno: {response.StatusCode}");
        }

        #endregion
    }
}
