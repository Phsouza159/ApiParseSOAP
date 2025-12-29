using System.Buffers.Text;
using System.Text;

namespace WebApiTeste.Autenticacao
{
    public static class Autenticacao
    {
        public static bool AuteticarBasic(string auth)
        {
            if(string.IsNullOrEmpty(auth))
                return false;

            auth = auth.Contains("Basic") ? auth.Replace("Basic", string.Empty).Trim() : auth;

            string[] parametros = Encoding.UTF8.GetString(Convert.FromBase64String(auth)).Split(":");

            if (parametros.Length != 2)
                return false;

            return parametros[0] == "admin" && parametros[1] == "admin";
        }
    }
}
