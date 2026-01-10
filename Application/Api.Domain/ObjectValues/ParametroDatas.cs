using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.ObjectValues
{
    public class ParametroDatas
    {
        public ParametroDatas()
        {
        }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }   

        public void ValidarPeriodo()
        {
            if (this.DataInicio > this.DataFim)
                throw new ArgumentException("Periodo inválido. Data inicio superior a data fim.");
        }
    }
}
