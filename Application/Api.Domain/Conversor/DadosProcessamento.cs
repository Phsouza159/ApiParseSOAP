using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Conversor
{
    public class DadosProcessamento
    {
        public DadosProcessamento()
        {
            this.ElementoImportado = string.Empty;
            this.TiposProcessador = Enum.TiposProcessadores.DEFAULT;
        }

        public Enum.TiposProcessadores TiposProcessador { get; set; }

        /// <summary>
        /// Definicao para registro obrigatorio
        /// </summary>
        public bool IsObrigatorio { get; set; }

        /// <summary>
        /// Definicao de registro como Propriedade
        /// </summary>
        public bool IsPropriedade { get; set; }


        public string ElementoImportado { get; set; }
    }
}
