using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Conversor
{
    public class DadosProcessamento
    {
        public Enum.TiposProcessadores TiposProcessador { get; set; }

        public string ElementoImportado { get; set; }
    }
}
