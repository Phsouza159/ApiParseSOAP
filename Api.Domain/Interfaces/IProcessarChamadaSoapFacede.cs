using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IProcessarChamadaSoapFacede
    {
        Schema CarregarDadosChamadaSoap(string servico, string xmlConteudo, string autenticacao);
        
        Task<ProcessamentoFluxoSoap> ProcessarFluxoSoap(Schema schema, IServicoLog servicoLog);
    }
}
