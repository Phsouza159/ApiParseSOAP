using Api.Domain.Configuracao;
using System.Net;
using System.Text.Json.Nodes;
using System.Xml;

namespace Api.Domain
{
    public class Schema
    {
        public XmlDocument Documento { get; internal set; }

        public XmlNode Corpo { get; private set; }

        public XmlDocument Contrato { get; internal set; }

        public List<XmlNode> XmlNodes { get; set; }
        
        public string NomeServico { get; private set; }

        public IEnumerable<Element> Element { get; set; }

        public string Resultado { get; internal set; }
        
        public HttpStatusCode Status { get; internal set; }
        
        public Servicos Servico { get; internal set; }

        public bool IsVazio { get; internal set; }

        public bool IsElementoEntrada { get; internal set; }

        internal void Carregar()
        {
            var doc = this.Documento;
            var nodes = doc.ChildNodes.Cast<XmlNode>().ToList();

            var envelope = nodes.First(e => e.Name == "soapenv:Envelope");

            if (envelope is null) throw new ArgumentException("Nao localizado - Envelope");

            var body = envelope.ChildNodes.Cast<XmlNode>().FirstOrDefault(e => e.Name == "soapenv:Body");

            if (body is null) throw new ArgumentException("Nao localizado - Body");

            var servico = body.ChildNodes[0];

            if (servico is null) throw new ArgumentException("Nao localizado - Servico");

            this.XmlNodes = servico.ChildNodes.Cast<XmlNode>().ToList();
            this.Corpo = body;
            this.NomeServico = servico.LocalName;
        }

        //internal List<Element> ProcessarElementos()
        //{
        //    List<Element> lista = new();

        //    foreach (var item in this.XmlNodes)
        //    {
        //        if (item != null)
        //        {
        //            Element element = this.ProcessarElemento(item);
        //            lista.Add(element);
        //        }
        //    }

        //    return lista;
        //}

        //internal Element ProcessarElemento(XmlNode item)
        //{
        //    string processador = this.RecuperarProcessadorNode(item);
        //    System.Enum.TryParse(processador, out Enum.TiposProcessadores tipoProcessador);
           
        //    Element element = new()
        //    {
        //        Nome = item.LocalName,
        //        Prefixo = item.Prefix,
        //        Valor = item.Value,
        //        Tipo = item.NodeType,
        //        Processador = tipoProcessador,
        //    };

        //    if (item.ChildNodes.Count > 0)
        //        element.No = item.ChildNodes.Cast<XmlNode>().Select(e => this.ProcessarElemento(e)).ToList();

        //    return element;
        //}

        //internal string RecuperarProcessadorNode(XmlNode item)
        //{
        //    if (item.NodeType == XmlNodeType.Element)
        //    {
        //        // Criar o namespace manager
        //        XmlNamespaceManager ns = new XmlNamespaceManager(this.Contrato.NameTable);
        //        ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

        //        var node = this.Contrato.SelectNodes($"//xs:element[@name='{this.NomeServico}']//xs:element[@name='{item.LocalName}']", ns);
        //        if (node != null && node.Count > 0)
        //        {
        //            var p = node.Item(0)?.Attributes?.GetNamedItem("type");
        //            if (p != null && p.Value != null)
        //                return p.Value.Replace("xs:", "");
        //        }
        //    }

        //    return string.Empty;
        //}

        //internal string ConverterParaJson()
        //{
        //    List<Element> lista = this.ProcessarElementos();
        //    JsonObject data = this.ProcessarElementos(lista);
        //    return data.ToJsonString();
        //}

        //internal JsonObject ProcessarElementos(List<Element> elementos)
        //{
        //    JsonObject json = new JsonObject();

        //    for (int i = 0; i < elementos.Count; i += 1)
        //    {
        //        Element elemento = elementos[i];
        //        this.ProcessarElementos(json, elemento);
        //    }

        //    return json;
        //}

        //internal void ProcessarElementos(JsonObject json, Element elemento)
        //{
        //    json[elemento.Nome] = JsonValue.Create(elemento.Converter());
        //}
    }
}
