using Api.Domain.Enum;

namespace Api.Domain.Helper
{
    public static class ProcessadoresHelper
    {

        private delegate object PROC_VALUE(string valor);

        private static PROC_VALUE[] Processadores { get; set; }

        private static bool IsProcessadorSucesso = ProcessadoresHelper.Processadores != null && ProcessadoresHelper.Processadores.Length > 0;

        #region CARREGAR PROCESSADORES

        private static void CarregarProcessadores()
        {
            if (ProcessadoresHelper.IsProcessadorSucesso)
                return;

            int length = System.Enum.GetValues(typeof(Enum.TiposProcessadores)).Length;
            ProcessadoresHelper.Processadores = new PROC_VALUE[length];

            #region PROCS 

            ProcessadoresHelper.Processadores[(short)TiposProcessadores.unsignedLong] = ConversorValorHelper.PRC_USINGNEDLONG;

            #endregion
        }

        #endregion

        public static object CarregarValorFormatado(this Element elemento, string valor)
        {
            ProcessadoresHelper.CarregarProcessadores();

            PROC_VALUE processador = ProcessadoresHelper.Processadores[(short)elemento.Processador.TiposProcessador];

            if (processador == null)
            {
                processador = ConversorValorHelper.PRC_DEFAULT;
            }

            return processador(valor);
        }
    }
}
