using Api.Domain;
using Api.Domain.Interfaces;

namespace Api.Application.Facede
{
    public class ServicoWebFacede : IServicoWebFacede
    {
        public ServicoWebFacede(IServicoWeb servicoWeb)
        {
            ServicoWeb = servicoWeb;
        }

        public IServicoWeb ServicoWeb { get; }

        /// <summary>
        /// Enviar Processamento Web
        /// </summary>
        public Task EnviarProcessamento(Schema schema, IServicoLog servicoLog)
        {
            return ServicoWeb.Enviar(schema, servicoLog);
        }
    }
}
