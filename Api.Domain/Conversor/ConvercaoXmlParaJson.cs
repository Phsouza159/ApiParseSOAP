using Api.Domain.Configuracao;
using Api.Domain.Helper;
using Api.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Api.Domain.Conversor
{
    public class ConvercaoXmlParaJson : IConvercaoXmlParaJson
    {
        public string ConverterParaJson(Schema schema)
        {
            List<Element> lista = this.Converter(schema);
            JsonObject data = this.ProcessarElementos(lista);
            //return data.ToJsonString();
            // TESTE
            return JsonConvert.SerializeObject(lista);
        }

        public List<Element> Converter(Schema schema)
        {
            List<Element> lista = new();
            // TODO: VALIDAR xs e xsd
            string prefixoBusca = "xsd";

            foreach (var item in schema.XmlNodes)
            {
                if (item != null)
                {
                    Element element = this.ProcessarElemento(schema, item, prefixoBusca);
                    lista.Add(element);
                }
            }

            return lista;
        }

        #region PROCESSAR XML PARA JSON

        internal Element ProcessarElemento(Schema schema, XmlNode item, string prefixoBusca)
        {
            //System.Enum.TryParse(processador, out Enum.TiposProcessadores tipoProcessador);

            Element element = new()
            {
                Nome = this.RecuperarNomeElemento(item),
                Prefixo = item.Prefix,
                Valor = string.IsNullOrEmpty(item.Value) ? string.Empty : item.Value,
                Tipo = item.NodeType,
            };

            this.RecuperarProcessadorNode(element, schema, item, prefixoBusca);

            // MODO : complexType

            if (element.Processador.TiposProcessador == Enum.TiposProcessadores.OBJETO_IMPORTADO)
            {
                // Criar o namespace manager
                XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
                ns.AddNamespace(prefixoBusca, "http://www.w3.org/2001/XMLSchema");

                string path = $"//{prefixoBusca}:complexType[@name='{element.Processador.ElementoImportado}']";
                var dados = this.RecuperarElementoImportacaoServico(schema, path, prefixoBusca);
                
                foreach (XmlNode d in dados)
                {
                    element.No.AddRange(this.ProcessarListaNoFilhos(schema, d.ChildNodes.Cast<XmlNode>(), prefixoBusca));
                }
            }

            // MODO : Simples
           else if (item.ChildNodes.Count > 0)
                element.No = item.ChildNodes.Cast<XmlNode>().Select(e => this.ProcessarElemento(schema, e, prefixoBusca)).ToList();

            return element;
        }

        private string RecuperarNomeElemento(XmlNode item)
        {
            var name = item?.Attributes?.GetNamedItem("name");

            if (name != null && name.Value != null)
                return name.Value;

            return item.LocalName;
        }

        #region RECUPERAR ELEMENTOS FILHOS

        internal List<Element> ProcessarListaNoFilhos(Schema schema, IEnumerable<XmlNode> xmlNodes, string prefixoBusca)
        {
            List<Element> elements = new List<Element>();

            foreach (var xmlNode in xmlNodes)
            {
                string[] itensIgnorar = new string[] {"annotation"};

                if (xmlNode.NodeType == XmlNodeType.Element && !itensIgnorar.Contains(xmlNode.LocalName))
                {
                    elements.Add(this.ProcessarElemento(schema, xmlNode, prefixoBusca));
                }
            }

            return elements;
        }

        #endregion

        internal void RecuperarProcessadorNode(Element element, Schema schema, XmlNode item, string prefixoBusca)
        {
            if (item.NodeType == XmlNodeType.Element)
            {
                var p = item?.Attributes?.GetNamedItem("type");
                if (p != null && p.Value != null && System.Enum.TryParse(p.Value.RecuperarParametro(":", 1), out Enum.TiposProcessadores tipoNode))
                {
                    element.Processador.TiposProcessador = tipoNode;
                    return;
                }

                // Criar o namespace manager
                XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
                ns.AddNamespace(prefixoBusca, "http://www.w3.org/2001/XMLSchema");

                var dados = this.RecuperarElementoServico(schema, item, ns, prefixoBusca);

                if(dados != null && dados.Count > 0)
                {
                    var node = this.RecuperarNodeElemento(item, dados);

                    if (node != null && node?.Attributes?.GetNamedItem("type") is XmlNode typeNode && typeNode != null && typeNode.Value != null)
                    {
                        if (this.IsElementoNodeEntity(typeNode))
                        {
                            element.Processador.ElementoImportado = typeNode.Value.RecuperarParametro(":", 1); ;
                            element.Processador.TiposProcessador = Enum.TiposProcessadores.OBJETO_IMPORTADO;
                        }
                        else 
                        {
                            System.Enum.TryParse(typeNode.Value.Split(":")[1], out Enum.TiposProcessadores tp);
                            element.Processador.TiposProcessador = tp;
                        }
                    }
                }
            }
        }

        private bool IsElementoNodeEntity(XmlNode typeNode)
        {
            string prefixoExtends = "bons1";
            return typeNode != null && typeNode.Value != null && typeNode.Value.Contains(":") && typeNode.Value.Split(":")[0] == prefixoExtends;

        }

        private XmlNode? RecuperarNodeElemento(XmlNode xmlNode, XmlNodeList xmlList)
        {
            foreach (XmlNode item in xmlList)
            {
                var p = item?.Attributes?.GetNamedItem("name");
                if (p != null && p.Value != null && p.Value.Contains(xmlNode.LocalName))
                    return item;

                return RecuperarNodeElemento(xmlNode, item.ChildNodes);
            }

            return null;
        }

        private XmlNodeList? RecuperarElementoServico(Schema schema, XmlNode item, XmlNamespaceManager ns, string prefixoBusca)
        {
            var dados = schema.Contrato.SelectNodes($"//{prefixoBusca}:element[@name='{schema.NomeServico}']", ns);

            if (dados != null && dados.Count == 0 && schema.Servico.IsImportacao)
            {
                string path = $"//{prefixoBusca}:element[@name='{item.LocalName}']";
                dados = this.RecuperarElementoImportacaoServico(schema, path, prefixoBusca);
            }

            return dados;
        }

        private XmlNodeList? RecuperarElementoImportacaoServico(Schema schema, string path, string prefixoBusca)
        {
            var servico = schema.Servico;

            for (int i = 0; i < servico.ConteudoArquivos.Count; i += 1)
            {
                XmlDocument importedDoc = new XmlDocument();
                var arquivoXml = servico.ConteudoArquivos.ElementAt(i);

                if (arquivoXml.Key.Contains(".xsd"))
                {
                    using (var stream = new MemoryStream(arquivoXml.Value))
                    using (var reader = XmlReader.Create(stream))
                    {
                        importedDoc.Load(reader);
                    }

                    XmlNamespaceManager nsImport = new XmlNamespaceManager(importedDoc.NameTable);
                    nsImport.AddNamespace(prefixoBusca, "http://www.w3.org/2001/XMLSchema");

                    var importedNodes = importedDoc.SelectNodes(path, nsImport);

                    if (importedNodes != null && importedNodes.Count > 0)
                    {
                        // aqui você pode retornar ou acumular os resultados
                        return importedNodes;
                    }
                }
            }

            return null;
        }


        #region ELEMENTO SIMPLES 

        // TODO : VALIDAR DEPOIS PARA ENTRADAS SIMPLES

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
