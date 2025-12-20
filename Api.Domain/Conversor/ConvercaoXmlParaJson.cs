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

            //TODO - VALIDAR
            // CRIAR FUNCAO PARA VALIDAR ELEMENTOS

            List<Element> elementosCorpo = this.TratarLista(lista);
            JsonObject data = this.ProcessarElementos(elementosCorpo);
            return data.ToJsonString();
        }

        private List<Element> TratarLista(List<Element> lista)
        {
            List<Element> listaTratada = new List<Element>();

            foreach (var item in lista)
            {
                var filhosFiltrados = TratarLista(item.No);

                if (item.IsPropriedade)
                {
                    Element novo = new Element();
                    item.Copiar(novo);
                    novo.No = filhosFiltrados;

                    listaTratada.Add(novo);
                }
                else
                {
                    listaTratada.AddRange(filhosFiltrados);
                }
            }

            return listaTratada;
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
                    element.IsPropriedade = true;
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
                Tipo = item.NodeType,
                IsPropriedade = item.LocalName == "element",
            };

            this.RecuperarValorPropriedade(element, schema);
            this.RecuperarProcessadorNode(element, schema, item, prefixoBusca);

            // MODO : complexType
            if (element.Processador.TiposProcessador == Enum.TiposProcessadores.OBJETO_IMPORTADO)
            {
                this.CarregarDodosElementosComplexo(element, schema, item, prefixoBusca);
            }
            // MODO : Extensions
            else if (element.Processador.TiposProcessador == Enum.TiposProcessadores.EXTENSION)
            {
                this.CarregarDodosElementosComplexo(element, schema, item, prefixoBusca);
                this.CarregarDadosElementoSimples(element, schema, item, prefixoBusca);
            }
            // MODO : Simples
            else
            {
                this.CarregarDadosElementoSimples(element, schema, item, prefixoBusca);
            }

            return element;
        }

        private void RecuperarValorPropriedade(Element element, Schema schema)
        {
            if (!element.IsPropriedade)
                return;

            string prefixoBusca = string.Empty;
            XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
            ns.AddNamespace(prefixoBusca, "http://www.w3.org/2001/XMLSchema");

            string path = $"//{element.Nome}";
            var p = schema.Corpo.SelectNodes(path, ns);

            if(p != null && p.Count == 1)
            {
                element.Valor = p[0].InnerText;
                return;
            }
        }

        private void CarregarDadosElementoSimples(Element element, Schema schema, XmlNode item, string prefixoBusca)
        {
            if(item.ChildNodes.Count > 0)
            {
                element.No.AddRange(item.ChildNodes.Cast<XmlNode>().Select(e => this.ProcessarElemento(schema, e, prefixoBusca)).ToList());
            }
        }

        private void CarregarDodosElementosComplexo(Element element, Schema schema, XmlNode item, string prefixoBusca)
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
                // VALORES RESERVADOS
                else if (p != null && p.Value != null && ConversorValorHelper.IsNomeReservado(p.Value.RecuperarParametro(":", 1)))
                {
                    element.Processador.TiposProcessador = ConversorValorHelper.RecuperarProcessadorReservado(p.Value.RecuperarParametro(":", 1));
                    return;
                }
                else if (this.IsElementoExtends(item))
                {
                    element.Processador.ElementoImportado = item.RecuperarAtributo("base").RecuperarParametro(":", 1);
                    element.Processador.TiposProcessador = Enum.TiposProcessadores.EXTENSION;
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

        private bool IsElementoExtends(XmlNode typeNode)
        {
            string prefixoExtends = "xsd";
            return typeNode != null && typeNode.Name == $"{prefixoExtends}:extension";
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
            var objet = elemento.Converter();

            if(objet is JsonValue data)
            {
                json[elemento.Nome] = data;
            }
            else if (objet is JsonObject dataObjeto)
            {
                json[elemento.Nome] = dataObjeto;

            }
            else
            {
                json[elemento.Nome] = JsonValue.Create(objet);
            }

        }

        #endregion
    }
}
