using Api.Domain.Api;
using Api.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Validacoes.Conversores
{
    [TestClass]
    public class ConversorXMLparaJSON : Base.BaseTeste
    {
        public ConversorXMLparaJSON()
        {
        }

        [TestMethod]
        [DataRow("WebBenchmark")]
        [DataRow("GEWSV0006_Banco")]
        public void TesteConversaoXml(string servico)
        {
            base.CarregarDadosContratosServicos();
            
            string conteudo = this.RecuperarConteudoTesteXML(servico);
            string esperado = this.RecuperarConteudoEsperadoJson(servico);

            var facede = base.Provider.GetRequiredService<IProcessarChamadaSoapFacede>();
            var conversor = base.Provider.GetRequiredService<IConvercaoXmlParaJson>();

            using Schema schema = facede.CarregarDadosChamadaSoap(servico, conteudo, autenticacao : string.Empty);

            string json = conversor.ConverterParaJson(schema);

            Assert.AreEqual(esperado, json);
        }

        private string RecuperarConteudoEsperadoJson(string servico)
        {
            switch (servico)
            {
                case "WebBenchmark":
                    return Arquivos.ArquivosRecursos.CONTEUDO_TESTE_WebBenchmark_JSON;

                case "GEWSV0006_Banco":
                    return Arquivos.ArquivosRecursos.CONTEUDO_TESTE_Banco_JSON;
            }

            Assert.Fail($"Serm suporte para o arquivo '{servico}' no modelo de arquivos JSON");
            return string.Empty;
        }

        private string RecuperarConteudoTesteXML(string servico)
        {
            switch (servico)
            {
                case "WebBenchmark":
                    return Arquivos.ArquivosRecursos.CONTEUDO_TESTE_WebBenchmark_XML;

                case "GEWSV0006_Banco":
                    return Arquivos.ArquivosRecursos.CONTEUDO_TESTE_Banco_XML;
            }

            Assert.Fail($"Serm suporte para o arquivo '{servico}' no modelo de arquivos XML");
            return string.Empty;
        }
    }
}
