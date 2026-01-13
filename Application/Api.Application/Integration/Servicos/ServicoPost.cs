using Api.Domain.Api;
using Api.Domain.Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Integration;
using Api.Domain.ObjectValues;
using System.Text;

namespace Api.Application.Integration.Servicos
{
    public class ServicoPost : ObjetoBase, IServicoPost
    {
        public ServicoPost()
        {
        }

        public async Task Executar(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            using var client = new HttpClient();

            // CARREGAR AUTENTICACAO
            schema.RecuperarContrato().Autenticacao?.CarregarAutenticacao(client, servicoLog);

            servicoLog.CriarLog(schema.Servico.Nome, $"REQUEST POST PARA: '{contrato.Api}'", TipoLog.INFO);

            var content = new StringContent(envelope.ConteudoEnvio, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await client.PostAsync(contrato.Api, content);

            servicoLog.CriarLog(schema.Servico.Nome, $"RETORNO POST PARA: '{contrato.Api}' - STATUS: '{response.StatusCode}' ", TipoLog.INFO);

            await TratarRetorno(schema, envelope, servicoLog, response);
        }

        private async Task TratarRetorno(Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog, HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new AutorizacaoException($"Sem autorização para chamada em '{schema.Servico.Nome}'.");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                envelope.ConteudoRetorno = await response.Content.ReadAsStringAsync();
                servicoLog.CriarLog(schema.Servico.Nome, envelope.ConteudoRetorno, TipoLog.RETORNO_JSON);
                return;
            }

            //TODO: TRATAR DEMAIS STATUS CODE

            throw new ArgumentException($"Sem tratamento para retorno: {response.StatusCode}");
        }
    }
}
