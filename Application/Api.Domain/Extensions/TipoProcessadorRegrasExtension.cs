using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Extensions
{
    internal static class TipoProcessadorRegrasExtension
    {

        public static bool RegraTipoProcessadorObjeto(this Enum.TiposProcessadores tipo)
        {
            return
                   tipo == Enum.TiposProcessadores.OBJETO
                || tipo == Enum.TiposProcessadores.OBJETO_IMPORTADO
                || tipo == Enum.TiposProcessadores.OBJETO_ARRAY;
        }
    }
}
