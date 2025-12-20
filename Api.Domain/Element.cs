
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
            this.No = new List<Element?>();
            this.Processador = new DadosProcessamento();
        }

        public string Nome { get; internal set; }
        public string Prefixo { get; internal set; }
        public string Valor { get; internal set; }
        public XmlNodeType Tipo { get; internal set; }
        public List<Element> No { get; internal set; }

        public bool IsPropriedade { get; internal set; }

        public DadosProcessamento Processador { get; set; }

        public object Converter()
        {
            string defaultNull = "null";

            switch (this.Processador.TiposProcessador)
            {

                case TiposProcessadores.OBJETO:
                case TiposProcessadores.OBJETO_IMPORTADO:
                    return this.ConverterObjeto();

                default:
                    return this.CarregarValorFormatado(this.Valor);
            }


            //// RECUPERAVAR VALOR #TEXT DO NO
            //if(this.Tipo == XmlNodeType.Element 
            //    && this.No.Count == 1
            //    && this.No.Any(e => e.Tipo == XmlNodeType.Text))
            //{
            //    var no = this.No.First();

            //    if (no.Valor is null)
            //        return defaultNull;

            //    return this.CarregarValorFormatado(no.Valor);
            //}
            //// TODO: AJUSTAR ARRAY LIST E OBJECT 
            //// RETORNANDO ARRAY LIST
            //else if (this.Tipo == XmlNodeType.Element
            //    && this.No.Count >= 1)
            //{
            //    List<object> lista = new List<object>();

            //    for (int i = 0; i < this.No.Count; i += 1)
            //    {
            //        Element element = this.No[i];
            //        lista.Add(element.Converter());
            //    }

            //    return lista;
            //}
        }

        private object ConverterObjeto()
        {
            JsonObject json = new JsonObject();

            foreach (var no in this.No)
            {
                json[no.Nome] = JsonValue.Create(no.Converter());
            }

            return json;
        }

        internal XmlElement ConverterXml(XmlDocument document, Schema schema)
        {
            XmlElement item = document.CreateElement(schema.Servico.Prefixo, $"{this.Nome}", schema.Servico.UrlHost);
            // item.SetAttribute(this.Nome, this.Valor);
            item.InnerText = this.Valor;

            return item;
        }

        internal void Copiar(Element item)
        {
            item.Nome = this.Nome;
            item.Valor = this.Valor;
            item.Processador = this.Processador;
            item.IsPropriedade = this.IsPropriedade;
        }
    }
}
