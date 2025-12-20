using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.ObjectValues
{
    internal class LogWeb
    {
        public string Nome { get; set; }

        public bool IsSucesso { get; set; }

        public Enum.TipoLog TipoLog { get; set; }

        public string Data { get; set; }
    }
}
