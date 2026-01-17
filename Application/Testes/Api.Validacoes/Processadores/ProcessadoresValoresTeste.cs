using Api.Domain.Api;
using Api.Domain.Enum;
using Api.Domain.Helper;

namespace Api.Validacoes.Processadores
{
    [TestClass]
    public class ProcessadoresValoresTeste : Base.BaseTeste
    {
        public ProcessadoresValoresTeste()
        {
        }

        [TestMethod]
        [DataRow(TiposProcessadores.UNSIGNEDLONG, "10", (UInt64)10)]
        [DataRow(TiposProcessadores.UNSIGNEDLONG, "10A", null)]
        [DataRow(TiposProcessadores.UNSIGNEDLONG, "-1", null)]
        [DataRow(TiposProcessadores.SHORT, "10", (short)10)]
        [DataRow(TiposProcessadores.SHORT, "10A", null)]
        [DataRow(TiposProcessadores.INT, "10", (int)10)]
        [DataRow(TiposProcessadores.INT, "10A", null)]
        [DataRow(TiposProcessadores.LONG, "10", (long)10)]
        [DataRow(TiposProcessadores.LONG, "10A", null)]
        [DataRow(TiposProcessadores.DATE, "2025-01-01T00:00:00", "2025-01-01")]
        [DataRow(TiposProcessadores.DATE, "TESTE", null)]
        [DataRow(TiposProcessadores.STRING, "TESTE", "TESTE")]
        public void TestarProc(TiposProcessadores tipoProcessador, string valor, object valorEsperado)
        {
            Elemento elemento = new Elemento();
            
            elemento.Processador.TiposProcessador = tipoProcessador;
            elemento.Valor = valor;

            object? valorConvertido = elemento.CarregarValorFormatado(elemento.Valor);

            if (valorEsperado is null)
                Assert.IsFalse(elemento.Notificacoes.IsValido, "Esperado objeto inválido.");
            else
                Assert.IsTrue(elemento.Notificacoes.IsValido, "Esperado objeto válido.");

            Assert.AreEqual(valorEsperado, valorConvertido, $"Falha ao converter valor. '{valor}' para '{valorEsperado}'");
        }

        [TestMethod]
        public void TestarProcDecimal( )
        {
            Elemento elemento = new Elemento();
            object valorEsperado = (decimal)1500000;

            elemento.Processador.TiposProcessador = TiposProcessadores.DECIMAL;
            elemento.Valor = "1.5e6m";

            object? valorConvertido = elemento.CarregarValorFormatado(elemento.Valor);

            Assert.AreEqual(valorEsperado, valorConvertido, $"Falha ao converter valor. '{elemento.Valor}' para '{valorEsperado}'");
        }
    }
}
