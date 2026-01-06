using Api.Domain.Api.Domain;

namespace Api.Domain.Interfaces.Repository
{
    public interface IRepositorioData : IDisposable
    {
        bool AdicionarRegistroLog(RegistroLog registro);

        void ConfgurarCaminhoData(string caminho);
    }
}
