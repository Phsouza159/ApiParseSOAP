using Api.Domain.Api;
using Api.Domain.Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Integration;
using Api.Domain.ObjectValues;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Application.Integration
{
    public class ServicoArquivo : ObjetoBase, IServicoArquivo
    {
        public async Task Executar(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
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
    }
}
