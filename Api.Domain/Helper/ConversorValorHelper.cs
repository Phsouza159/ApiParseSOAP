
namespace Api.Domain.Helper
{
    public static class ConversorValorHelper
    {
        internal static bool IsNomeReservado(string nome)
        {
            switch (nome.ToLower())
            {
                case "string":
                case "short":
                    return true;

                default:
                    return false;
            }
        }

        internal static Enum.TiposProcessadores RecuperarProcessadorReservado(string nome)
        {
            switch (nome.ToLower())
            {
                case "string":
                    return Enum.TiposProcessadores.STRING;
                case "short":
                    return Enum.TiposProcessadores.SHORT;

                default:
                    return Enum.TiposProcessadores.DEFAULT;
            }
        }

        internal static object PRC_DEFAULT(string valor)
        {
            return !string.IsNullOrEmpty(valor) ? valor.ToString() : string.Empty;
        }

        internal static object PRC_SHORT(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return null;

            if(short.TryParse(valor, out short _short))
            {
                return _short;
            }

            // TDOO: VALIDAR EXECEPTIONS PARA CAST DE VALOR
            throw new ArgumentException();
        }

        internal static object PRC_INTEGER(string valor)
        {
            if (valor is null)
                return null;

            if(int.TryParse(valor, out int _int))
            {
                return _int;
            }

            // CONVERTER PARA TIPO SUPERIOR
            return PRC_LONG(valor);
           //throw ProcessarExecption(valor);
        }

        internal static object PRC_LONG(string valor)
        {
            if (valor is null)
                return null;

            if (long.TryParse(valor, out long _int))
            {
                return _int;
            }

            throw ProcessarExecption(valor);
        }


        internal static object PRC_USINGNEDLONG(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return null;

            if (ulong.TryParse(valor, out ulong _unlog))
            {
                return _unlog;
            }

            // TDOO: VALIDAR EXECEPTIONS PARA CAST DE VALOR
            throw new ArgumentException();
        }

        internal static string PRC_STRING(string valor)
        {
            if(valor is null)
                return null;

            return valor.ToString();
        }

        #region EXCEPTION

        internal static Exception ProcessarExecption(object valor)
        {
            return new ArgumentException($"Tipo não suportado para Conversão: {valor}");
        }

        #endregion

    }
}
