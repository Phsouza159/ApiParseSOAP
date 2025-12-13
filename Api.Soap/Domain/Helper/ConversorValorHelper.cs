
namespace ApiParseSOAP.Domain.Helper
{
    public static class ConversorValorHelper
    {
        internal static object PRC_DEFAULT(string valor)
        {
            return valor.ToString();
        }

        internal static object PRC_USINGNEDLONG(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                return null;
            }

            if (ulong.TryParse(valor, out ulong _unlog))
            {
                return _unlog;
            }

            // TDOO: VALIDAR EXECEPTIONS PARA CAST DE VALOR
            throw new ArgumentException();
        }
    }
}
