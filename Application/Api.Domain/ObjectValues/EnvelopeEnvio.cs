using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.ObjectValues
{
    public class EnvelopeEnvio
    {
        public EnvelopeEnvio()
        {
            this.ConteudoEnvio = string.Empty;
            this.ConteudoRetorno = string.Empty;
        }
        public string ConteudoEnvio { get; set;}

        public string ConteudoRetorno { get; set; }

    }
}
