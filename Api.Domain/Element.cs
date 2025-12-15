
using Api.Domain.Enum;
using Api.Domain.Helper;
using System.Xml;

namespace Api.Domain
{
    public class Element
    {
        public Element()
        {
            this.No = new List<Element?>();
        }

        public string Nome { get; internal set; }
        public string Prefixo { get; internal set; }
        public string Valor { get; internal set; }
        public XmlNodeType Tipo { get; internal set; }
        public List<Element> No { get; internal set; }
        public TiposProcessadores Processador { get; internal set; }

        public object Converter()
        {
            string defaultNull = "null";

            // RECUPERAVAR VALOR #TEXT DO NO
            if(this.Tipo == XmlNodeType.Element 
                && this.No.Count == 1
                && this.No.Any(e => e.Tipo == XmlNodeType.Text))
            {
                var no = this.No.First();

                if (no.Valor is null)
                    return defaultNull;

                return this.CarregarValorFormatado(no.Valor);
            }
            // TODO: AJUSTAR ARRAY LIST E OBJECT 
            // RETORNANDO ARRAY LIST
            else if (this.Tipo == XmlNodeType.Element
                && this.No.Count >= 1)
            {
                List<object> lista = new List<object>();

                for (int i = 0; i < this.No.Count; i += 1)
                {
                    Element element = this.No[i];
                    lista.Add(element.Converter());
                }

                return lista;
            }

            return defaultNull;
        }
    }
}
