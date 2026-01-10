using Api.Domain.Api;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using System.Text;

namespace Api.Application.Facede
{
    public class ServicoWebFacede : IServicoWebFacede
    {
        public ServicoWebFacede(IServicoIntegracao servicoWeb, IConvercaoXmlParaJson convercaoXmlParaJson)
        {
            ServicoWeb = servicoWeb;
            ConvercaoXmlParaJson = convercaoXmlParaJson;
        }

        public IServicoIntegracao ServicoWeb { get; }

        public IConvercaoXmlParaJson ConvercaoXmlParaJson { get; }

        /// <summary>
        /// Enviar Processamento Web
        /// </summary>
        public async Task<EnvelopeEnvio> EnviarProcessamento(Schema schema, IServicoLog servicoLog)
        {
            EnvelopeEnvio envelope = new EnvelopeEnvio();

            envelope.ConteudoEnvio = this.ConvercaoXmlParaJson.ConverterParaJson(schema);
            servicoLog.CriarLog(schema.Servico.Nome, envelope.ConteudoEnvio, TipoLog.CHAMADA_JSON);
          

            await ServicoWeb.Enviar(schema, envelope, servicoLog);

            return envelope;
        }
    }
}
