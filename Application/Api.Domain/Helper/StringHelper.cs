using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Api.Domain.Helper
{
    public static class StringHelper
    {
        public static string RecuperarAtributo(this XmlNode xmlNode, string nomeAtributo)
        {
            var atributo = xmlNode.Attributes?.GetNamedItem(nomeAtributo);
            return atributo != null && !string.IsNullOrEmpty(atributo.Value) ? atributo.Value : string.Empty;
        }

        public static string RecuperarParametro(this string texto, string separador, int posicao)
        {
           return texto.Contains(separador) ? texto.Split(":")[posicao] : texto;
        }

        public static string ConcatenarUrl(string baseUrl, string path)
        {
            baseUrl = baseUrl.TrimEnd('/');
            path = path.TrimStart('/');

            // Junta com uma única barra
            return $"{baseUrl}/{path}";
        }
    }
}
