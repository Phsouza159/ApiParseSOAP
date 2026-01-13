using Api.Domain.Api;
using Api.Domain.Configuracao;
using Api.Domain.ObjectValues;

namespace Api.Domain.Interfaces.Integration
{
    public interface IntegrationBase : INotificacoes, IDisposable
    {
        Task Executar(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog);
    }
}
