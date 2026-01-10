
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
                case "int":
                case "decimal":
                case "date":
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
                case "int":
                    return Enum.TiposProcessadores.INTEGER;
                case "decimal":
                    return Enum.TiposProcessadores.DECIMAL;
                case "date":
                    return Enum.TiposProcessadores.DATE;

                default:
                    return Enum.TiposProcessadores.DEFAULT;
            }
        }

        #region PROC

        internal static object PRC_DEFAULT(object valor)
        {
            throw new ArgumentException("Sem suporte");

            if (valor is null)
                return null;

            return valor.ToString();
        }

        internal static object PRC_SHORT(object valor)
        {
            if (valor is null)
                return null;

            if(short.TryParse(valor.ToString(), out short _short))
            {
                return _short;
            }

            // TDOO: VALIDAR EXECEPTIONS PARA CAST DE VALOR
            throw new ArgumentException();
        }

        internal static object PRC_INTEGER(object valor)
        {
            if (valor is null)
                return null;

            if(int.TryParse(valor.ToString(), out int _int))
            {
                return _int;
            }

            // CONVERTER PARA TIPO SUPERIOR
            return PRC_LONG(valor);
           //throw ProcessarExecption(valor);
        }

        internal static object PRC_DECIMAL(object valor)
        {
            if (valor is null)
                return null;

            if (decimal.TryParse(valor.ToString(), out decimal _decimal))
            {
                return _decimal;
            }

            // CONVERTER PARA TIPO SUPERIOR
            //return PRC_LONG(valor);
            throw ProcessarExecption(valor, typeof(decimal));
        }

        internal static object PRC_LONG(object valor)
        {
            if (valor is null)
                return null;

            if (long.TryParse(valor.ToString(), out long _int))
            {
                return _int;
            }

            throw ProcessarExecption(valor, typeof(long));
        }

        internal static object PRC_DATE(object valor)
        {
            if (valor is null)
                return null;

            if (DateTime.TryParse(valor.ToString(), out DateTime _date))
            {
                return _date.ToString("yyyy-MM-dd");
            }

            throw ProcessarExecption(valor, typeof(DateTime));
        }

        internal static object PRC_USINGNEDLONG(object valor)
        {
            if (valor is null)
                return null;

            if (ulong.TryParse(valor.ToString(), out ulong _unlog))
            {
                return _unlog;
            }

            // TDOO: VALIDAR EXECEPTIONS PARA CAST DE VALOR
            throw new ArgumentException();
        }

        internal static string PRC_STRING(object valor)
        {
            if(valor is null)
                return null;

            return valor.ToString();
        }

        #endregion

        #region EXCEPTION

        internal static Exception ProcessarExecption(object valor, Type type)
        {
            return new ArgumentException($"Valor '{valor}' não suportado para conversão do tipo '{type.Name}'.");
        }

        #endregion

    }
}
