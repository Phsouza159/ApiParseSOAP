using Api.Application.Integration.Servicos;
using Api.Domain.Api;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Integration;
using Api.Domain.ObjectValues;

namespace Api.Application.Integration
{
    public class ServicoIntegracao : IServicoIntegracao
    {
        private IServicoPost ServicoPost { get; }
        private IServicoArquivo ServicoArquivo { get; }
        private IServicoProcessadoresNode ServicoProcessadoresNode { get; }

        private IntegrationBase ServiceIntegration { get; set; }    

        public ServicoIntegracao(
                  IServicoPost servicoPost
                , IServicoArquivo servicoArquivo
                , IServicoProcessadoresNode servicoProcessadoresNode
            )
        {
            this.ServicoPost = servicoPost;
            this.ServicoArquivo = servicoArquivo;
            this.ServicoProcessadoresNode = servicoProcessadoresNode;
           
            // DEFAULT 
            this.ServiceIntegration = new ServicoNotImplementation();
        }

        public async Task Enviar(Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            var contrato = schema.RecuperarContrato();

            if (contrato != null && System.Enum.TryParse(contrato.Tipo, out TipoIntegracao tipoIntegracao))
            {
                switch (tipoIntegracao)
                {
                    case TipoIntegracao.POST:
                        this.ServiceIntegration = this.ServicoPost;
                        break;

                    case TipoIntegracao.FILE:
                        this.ServiceIntegration = this.ServicoArquivo;
                        break;

                    case TipoIntegracao.PROCESSADOR_NODE:
                        this.ServiceIntegration = this.ServicoProcessadoresNode;
                        break;
                }

                await this.ServiceIntegration.Executar(contrato, schema, envelope, servicoLog);
                return;
            }

            throw new ArgumentException($"Sem implementação para integração.");
        }
    }
}
