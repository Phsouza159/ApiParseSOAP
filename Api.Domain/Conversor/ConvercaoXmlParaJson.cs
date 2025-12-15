using Api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;

namespace Api.Domain.Conversor
{
    public class ConvercaoXmlParaJson : IConvercaoXmlParaJson
    {
        public string ConverterParaJson(Schema schema)
        {
            List<Element> lista = this.Converter(schema);
            JsonObject data = this.ProcessarElementos(lista);
            return data.ToJsonString();
        }

        public List<Element> Converter(Schema schema)
        {
            List<Element> lista = new();

            foreach (var item in schema.XmlNodes)
            {
                if (item != null)
                {
                    Element element = this.ProcessarElemento(schema, item);
                    lista.Add(element);
                }
            }

            return lista;
        }

        #region PROCESSAR XML PARA JSON

        internal Element ProcessarElemento(Schema schema, XmlNode item)
        {
            string processador = this.RecuperarProcessadorNode(schema, item);
            System.Enum.TryParse(processador, out Enum.TiposProcessadores tipoProcessador);

            Element element = new()
            {
                Nome = item.LocalName,
                Prefixo = item.Prefix,
                Valor = item.Value,
                Tipo = item.NodeType,
                Processador = tipoProcessador,
            };

            if (item.ChildNodes.Count > 0)
                element.No = item.ChildNodes.Cast<XmlNode>().Select(e => this.ProcessarElemento(schema, e)).ToList();

            return element;
        }

        internal string RecuperarProcessadorNode(Schema schema, XmlNode item)
        {
            if (item.NodeType == XmlNodeType.Element)
            {
                // Criar o namespace manager
                XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
                ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

                var node = schema.Contrato.SelectNodes($"//xs:element[@name='{schema.NomeServico}']//xs:element[@name='{item.LocalName}']", ns);
                if (node != null && node.Count > 0)
                {
                    var p = node.Item(0)?.Attributes?.GetNamedItem("type");
                    if (p != null && p.Value != null)
                        return p.Value.Replace("xs:", "");
                }
            }

            return string.Empty;
        }

        #endregion

        #region PROCESSAR ELEMENTOS PARA JSON

        internal JsonObject ProcessarElementos(List<Element> elementos)
        {
            JsonObject json = new JsonObject();

            for (int i = 0; i < elementos.Count; i += 1)
            {
                Element elemento = elementos[i];
                this.ProcessarElementos(json, elemento);
            }

            return json;
        }

        internal void ProcessarElementos(JsonObject json, Element elemento)
        {
            json[elemento.Nome] = JsonValue.Create(elemento.Converter());
        }

        #endregion
    }
}
