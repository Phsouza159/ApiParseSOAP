using Api.Domain.Api;
using Api.Domain.Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Integration;
using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Integration
{
    public class ServicoPost : ObjetoBase, IServicoPost 
    {
        public ServicoPost()
        {
        }

        #region

        public async Task Executar(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
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
