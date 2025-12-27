using Api.Domain.Extensions;
using Api.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Api.Domain.Conversor.Base
{
    public abstract class Conversor : IDisposable
    {
        #region VALIDAR TIPOS

        internal bool IsItemOutput(XmlNode xmlNode) => xmlNode != null && xmlNode.LocalName.Equals("output");

        #endregion

        #region RECUPERAR NODE - ITEM PAI

        /// <summary>
        /// Recuperar Item Pai do NO com base no nome do ITEM
        /// </summary>
        internal XmlNode? RecuperarItemPaiNode(XmlNode xmlNode, string nomeItem, XmlNodeType type = XmlNodeType.Element)
        {
            if (xmlNode is null || xmlNode.ParentNode is null)
                return null;

            foreach (XmlNode node in xmlNode.ParentNode)
            {
                if (node.NodeType == type && node.LocalName.Equals(nomeItem))
                    return node;
            }

            return null;
        }

        #endregion

        #region PROCESSAR ELEMENTO

        /// <summary>
        /// Processar o Elemento do NO e elementos filhos
        /// </summary>
        internal Element ProcessarElemento(Schema schema, XmlNode item, string prefixoBusca = "")
        {
            Element element = new()
            {
                Nome = this.RecuperarNomeElemento(item),
                Prefixo = item.Prefix,
                Tipo = item.NodeType,
                IsPropriedade = this.DefinirPropriedadeElemento(item, schema),
            
            };

            if (string.IsNullOrEmpty(prefixoBusca))
                prefixoBusca = schema.Servico.Prefixo;

            this.RecuperarValorPropriedade(element, schema);
            this.RecuperarProcessadorNode(element, schema, item, prefixoBusca);

            // MODO : complexType
            if (element.Processador.TiposProcessador.RegraTipoProcessadorObjeto())
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

        private bool DefinirPropriedadeElemento(XmlNode item, Schema schema)
        {
            if(item.NodeType == XmlNodeType.Element)
            {
                return item.LocalName == "element"; 
            }

            return false;
        }

        #endregion

        #region RECUPERAR VALOR

        /// <summary>
        /// RECUPERAR VALORES DAS PROPRIEDADES
        /// </summary>
        private void RecuperarValorPropriedade(Element element, Schema schema)
        {
            if (!element.IsPropriedade)
                return;

            string prefixoBusca = string.Empty;
            XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
            ns.AddNamespace(prefixoBusca, "http://www.w3.org/2001/XMLSchema");

            string path = $"//{element.Nome}";
            var p = schema.Corpo.SelectNodes(path, ns);

            if (p != null && p.Count == 1)
            {
                element.Valor = p[0].InnerText;
                return;
            }
        }

        #endregion

        #region TIPO SIMPLES

        /// <summary>
        /// PROCESSAR ELEMENTO DE TIPO SIMPLES
        /// </summary>
        private void CarregarDadosElementoSimples(Element element, Schema schema, XmlNode item, string prefixoBusca)
        {
            if (item.ChildNodes.Count > 0)
            {
                element.No.AddRange(item.ChildNodes.Cast<XmlNode>().Select(e => this.ProcessarElemento(schema, e, prefixoBusca)).ToList());
            }
        }

        #endregion

        #region TIPO COMPLEXO

        /// <summary>
        /// PROCESSAR ELEMENTOS DE TIPO COMPLEXO
        /// </summary>
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

        #endregion

        #region RECUPERAR NOME

        /// <summary>
        /// RECUPERAR NOME DO ELEMENTO
        /// </summary>
        private string RecuperarNomeElemento(XmlNode item)
        {
            var name = item?.Attributes?.GetNamedItem("name");

            if (name != null && name.Value != null)
                return name.Value;

            return item.LocalName;
        }

        #endregion

        #region RECUPERAR ELEMENTOS FILHOS

        internal List<Element> ProcessarListaNoFilhos(Schema schema, IEnumerable<XmlNode> xmlNodes, string prefixoBusca)
        {
            List<Element> elements = new List<Element>();

            foreach (var xmlNode in xmlNodes)
            {
                string[] itensIgnorar = new string[] { "annotation" };

                if (xmlNode.NodeType == XmlNodeType.Element && !itensIgnorar.Contains(xmlNode.LocalName))
                {
                    elements.Add(this.ProcessarElemento(schema, xmlNode, prefixoBusca));
                }
            }

            return elements;
        }

        #endregion

        #region PROCESSAR NO [SIMPLES/COMPLEXO]

        /// <summary>
        /// PROCESSAR 'NO' - PROCESSAR ELEMENTOS COMPLEXOS E SIMPLES
        /// </summary>
        internal void RecuperarProcessadorNode(Element element, Schema schema, XmlNode item, string prefixoBusca)
        {
            if (item.NodeType != XmlNodeType.Element)
                return;

            // RESOLVER ITEM SIMPLES
            bool itemProcessado = this.RecuperarProcessadorNodeSimples(element, schema, item, prefixoBusca);

            if (!itemProcessado)
            {
                // RESOLVER ITEM COMPLEXO
                this.RecuperarProcessadorNodeComplexo(schema,element, item, prefixoBusca);
            }
        }

        /// <summary>
        /// PROCESSAR ITENS SIMPLES
        /// </summary>
        private bool RecuperarProcessadorNodeSimples(Element element, Schema schema, XmlNode item, string prefixoBusca)
        {
            var p = item?.Attributes?.GetNamedItem("type");

            if (p != null && p.Value != null && System.Enum.TryParse(p.Value.RecuperarParametro(":", 1).ToUpper(), out Enum.TiposProcessadores tipoNode))
            {
                element.Processador.TiposProcessador = tipoNode;
                return true;
            }
            // VALORES RESERVADOS
            else if (p != null && p.Value != null && ConversorValorHelper.IsNomeReservado(p.Value.RecuperarParametro(":", 1)))
            {
                element.Processador.TiposProcessador = ConversorValorHelper.RecuperarProcessadorReservado(p.Value.RecuperarParametro(":", 1));
                return true;
            }
            else if (this.IsElementoExtends(item))
            {
                element.Processador.ElementoImportado = item.RecuperarAtributo("base").RecuperarParametro(":", 1);
                element.Processador.TiposProcessador = Enum.TiposProcessadores.EXTENSION;
                return true;
            }

            return false;
        }

        /// <summary>
        /// PROCESSAR ITENS COMPLEXO
        /// </summary>
        private void RecuperarProcessadorNodeComplexo(Schema schema, Element element, XmlNode item, string prefixoBusca)
        {
            // Criar o namespace manager
            XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
            ns.AddNamespace(prefixoBusca, "http://www.w3.org/2001/XMLSchema");

            // TODO: VALIDAR QUANDO NECESSARIO RECUPERAR DO NOME DO SERVICO
            var dados = this.RecuperarElementoServico(schema, item, ns, prefixoBusca);

            // DADO DO SERVICO CARREGADO
            if (dados != null && dados.Count > 0)
            {
                // RECUPERAR ITEM DA LISTA DE DADOS
                var node = this.RecuperarNodeElemento(schema, item, dados);
                this.CarregarTipoDoNoComplexo(schema, element, node);
            }
            // CARREGAR DO ITEM DIRETO
            else
            {
                this.CarregarTipoDoNoComplexo(schema, element, item);
            }
        }
            
        internal void CarregarTipoDoNoComplexo(Schema schema, Element element, XmlNode? node)
        {
            if (node != null && node?.Attributes?.GetNamedItem("type") is XmlNode typeNode && typeNode != null && typeNode.Value != null)
            {
                if (this.IsElementoArrayList(node))
                {
                    element.Processador.ElementoImportado = typeNode.Value.RecuperarParametro(":", 1); ;
                    element.Processador.TiposProcessador = Enum.TiposProcessadores.OBJETO_ARRAY;
                }
                else if (this.IsElementoNodeEntity(schema, typeNode))
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

        #endregion

        #region RESOLVER TIPO ELEMENTOS XML

        internal bool IsElementoExtends(XmlNode typeNode)
        {
            string prefixoExtends = "xsd";
            return typeNode != null && typeNode.Name == $"{prefixoExtends}:extension";
        }

        internal bool IsElementoNodeEntity(Schema schema, XmlNode typeNode)
        {
            if (typeNode is null || typeNode.Value is null) 
                return false;

            string pattern = schema.Servico.PrefixoImportacaoRegex;

            if (string.IsNullOrEmpty(pattern))
                pattern = @"^bons\d+$";

            string type = typeNode.Value.RecuperarParametro(":", 0);
            return Regex.IsMatch(type, pattern);
        }

        internal bool IsElementoArrayList(XmlNode typeNode)
        {
            if (typeNode is null || typeNode.Attributes is null)
                return false;

            var maxOcorrencia = typeNode.RecuperarAtributo("maxOccurs");
            return !string.IsNullOrEmpty(maxOcorrencia) && maxOcorrencia.ToLower().Equals("unbounded");
        }


        #endregion

        #region RECUPEPAR ELEMENTO LISTA

        /// <summary>
        /// RECUPERAR ELEMENTO DA LISTA COM BASE NO LOCAL NAME 
        /// </summary>
        private XmlNode? RecuperarNodeElemento(Schema schema, XmlNode xmlNode, XmlNodeList xmlList)
        {
            if (xmlList != null && xmlList.Count > 0)
            {
                // CORRIGIR ITEM
                for (int i = 0; i < xmlList.Count; i++)
                {
                    var dado = xmlList[i];
                    // LOCALIZAR ITEM DO SERVICO
                    if (this.GetNoPaiAttributo(schema, dado))
                    {
                        return dado;
                    }
                }
            }

            //foreach (XmlNode item in xmlList)
            //{
            //    var p = item?.Attributes?.GetNamedItem("name");
            //    if (p != null && p.Value != null && p.Value.Contains(xmlNode.LocalName))
            //        return item;

            //    return RecuperarNodeElemento(xmlNode, item.ChildNodes);
            //}

            return null;
        }

        #endregion

        #region RECUPERAR ELEMENTO COMPLEXO

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


        internal bool GetNoPaiAttributo(Schema schema, XmlNode xmlNode)
        {
            if (this.IsAttributosServico(xmlNode, schema))
                return true;

            if (xmlNode.ParentNode != null)
                return this.GetNoPaiAttributo(schema, xmlNode.ParentNode);

            return false;
        }

        internal bool IsAttributosServico(XmlNode xmlNode, Schema schema)
        {
            if (xmlNode is null)
                return false;

            var attr = xmlNode.Attributes?.GetNamedItem("name");
            if (attr != null)
            {
                if (attr.Value == schema.NomeServico)
                    return true;
            }

            return false;
        }

        #endregion

        #region PROCESSAR ELEMENTO IMPORTADO [XSD]

        /// <summary>
        /// Recuperar Node - Arquivo XSD Importado
        /// </summary>
        internal XmlNodeList? RecuperarElementoImportacaoServico(Schema schema, string path, string prefixoBusca = "")
        {
            var servico = schema.Servico;

            if (string.IsNullOrEmpty(prefixoBusca))
                prefixoBusca = schema.Servico.Prefixo;

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

                    var nodeImportado = importedDoc.SelectNodes(path, nsImport);

                    if (nodeImportado != null && nodeImportado.Count > 0)
                    {
                        return nodeImportado;
                    }
                }
            }

            return null;
        }

        #endregion

        #region TRATAR LISTA

        /// <summary>
        /// TRATAR LISTA - RECUPERAR ELEMENTOS DE PROPRIEDADE
        /// </summary>
        internal List<Element> TratarLista(List<Element> lista)
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

        #endregion

        internal bool IsDisponse { get; set; }

        public void Dispose()
        {
            if(!this.IsDisponse)
            {
                this.IsDisponse = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
