using Api.Domain.Api.Domain;
using Api.Domain.ObjectValues;

namespace Api.Domain.Interfaces.Repository
{
    public interface IRepositorioData : IDisposable
    {
        bool AdicionarRegistroLog(RegistroLog registro);

        void ConfgurarCaminhoData(string caminho);
        IEnumerable<RegistroLog> RecuperarRegistros(ParametroDatas parametro);
    }
}
