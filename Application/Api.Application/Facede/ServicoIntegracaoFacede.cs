using Api.Domain.Api;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;

namespace Api.Application.Facede
{
    public class ServicoIntegracaoFacede : IServicoIntegracaobFacede
    {
        public ServicoIntegracaoFacede(IServicoIntegracao servicoIntegracao, IConvercaoXmlParaJson convercaoXmlParaJson)
        {
            ServicoIntegracao = servicoIntegracao;
            ConvercaoXmlParaJson = convercaoXmlParaJson;
        }

        public IServicoIntegracao ServicoIntegracao { get; }

        public IConvercaoXmlParaJson ConvercaoXmlParaJson { get; }

        public async Task<EnvelopeEnvio> EnviarProcessamento(Schema schema, IServicoLog servicoLog)
        {
            EnvelopeEnvio envelope = new EnvelopeEnvio();

            envelope.ConteudoEnvio = this.ConvercaoXmlParaJson.ConverterParaJson(schema);
            servicoLog.CriarLog(schema.Servico.Nome, envelope.ConteudoEnvio, TipoLog.CHAMADA_JSON);

            await ServicoIntegracao.Enviar(schema, envelope, servicoLog);

            return envelope;
        }
    }
}
