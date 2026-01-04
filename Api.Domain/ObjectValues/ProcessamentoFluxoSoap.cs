using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.ObjectValues
{
    public class ProcessamentoFluxoSoap
    {
        public ProcessamentoFluxoSoap()
        {
            this.Conteudo = string.Empty;
            this.TipoConteudo = string.Empty;
        }

        public string Conteudo { get; set; }

        public string TipoConteudo { get; set; }
    }
}
