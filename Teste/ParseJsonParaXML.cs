using Api.Domain.Configuracao;

namespace Teste
{
    public class ParseJsonParaXML : Base.BaseTeste
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Servicos servicos = base.CarregarArquivoSchema("GEWSV0006_Banco");
            //servicos
        }
    }
}