
using Api.Domain.Conversor;
using Api.Domain.Enum;
using Api.Domain.Helper;
using System.Text.Json.Nodes;
using System.Xml;

namespace Api.Domain
{
    public class Element
    {
        public Element()
        {
            this.No = [];
            this.Processador = new DadosProcessamento();
        }

        public string Nome { get; set; }
        public string Prefixo { get; set; }
        public string Valor { get; set; }
        public XmlNodeType Tipo { get; set; }
        public List<Element> No { get; set; }

        public bool IsPropriedade { get; set; }

        public DadosProcessamento Processador { get; set; }


        internal void Copiar(Element item)
        {
            item.Nome = this.Nome;
            item.Valor = this.Valor;
            item.Processador = this.Processador;
            item.IsPropriedade = this.IsPropriedade;
            item.Tipo = this.Tipo;
        }
    }
}
