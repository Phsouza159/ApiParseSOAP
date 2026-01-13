using Api.Domain.Api;
using Api.Domain.Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Integration;
using Api.Domain.ObjectValues;

namespace Api.Application.Integration.Servicos
{
    internal class ServicoNotImplementation : ObjetoBase, IServicoNotImplementation
    {
        public Task Executar(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            throw new NotImplementedException($"Tipo de integração sem suporte '{contrato.Tipo}'.");
        }
    }
}
