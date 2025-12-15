using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Helper
{
    public static class StringHelper
    {
        public static string ConcatenarUrl(string baseUrl, string path)
        {
            baseUrl = baseUrl.TrimEnd('/');
            path = path.TrimStart('/');

            // Junta com uma única barra
            return $"{baseUrl}/{path}";
        }
    }
}
