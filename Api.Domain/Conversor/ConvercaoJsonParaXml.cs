using Api.Domain.Conversor.Extensions;
using Api.Domain.Enum;
using Api.Domain.Helper;
using Api.Domain.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;
using System.Web.Helpers;
using System.Xml;
using System.Xml.Linq;

namespace Api.Domain.Conversor
{
    public class ConvercaoJsonParaXml : Base.Conversor, IConvercaoJsonParaXml
    {

        public string ConverterParaXml(Schema schema)
        {
            List<Element> lista = this.ConverterContrato(schema);
            var contratoRetorno = this.TratarLista(lista);

            XmlDocument documento = this.CriarDocumentoXml(schema, contratoRetorno);

           // return JsonConvert.SerializeObject(contratoRetorno);

            //XmlDocument data = this.ProcessarElementos(schema, lista);
            return documento.OuterXml;
        }


        #region CRIAR DOCUMENTO XML


        internal XmlDocument CriarDocumentoXml(Schema schema, List<Element> listaContrato)
        {
            XmlDocument document = new XmlDocument();
            string prefixo = schema.Servico.Prefixo;

            // Criar Envelope com namespace SOAP
            XmlElement envelope = document.CreateElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            document.AppendChild(envelope);

            // Criar Body
            XmlElement body = document.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            envelope.AppendChild(body);

            JToken token = JToken.Parse(schema.Resultado);

            foreach (var elemento in listaContrato)
            {
                string caminhoItem = "";
                this.ProcessarListaElement(schema, document, body, elemento, token, caminhoItem);
            }

            return document;
        }


        internal void ProcessarListaElement(Schema schema, XmlDocument document, XmlElement documentoAppend, Element elemento, JToken token, string caminhoItem)
        {
            XmlElement item = document.CreateElement($"{elemento.Nome}");

            //item.InnerText =
            caminhoItem = string.IsNullOrEmpty(caminhoItem) ? elemento.Nome : $"{caminhoItem}.{elemento.Nome}";

            var valorToken = token.SelectToken(caminhoItem);
            this.CarregarPropriedadeEValidacoes(item, valorToken, elemento, caminhoItem);

            // PROCESSAR OBJETO ARRAY
            if (elemento.Processador.TiposProcessador == TiposProcessadores.OBJETO_ARRAY)
            {
                this.ProcessarListaElementArray(schema, document, item, documentoAppend, elemento, token, valorToken, caminhoItem);
            }
            // PROCESSAR ITEM SIMPLES
            else
            {
                this.ProcessarListaElementSimples(schema, document, item, documentoAppend, elemento, token, caminhoItem);
            }
        }

        #region PROCESSAR ITEM ARRAY

        internal void ProcessarListaElementArray(Schema schema
            , XmlDocument document
            , XmlElement item
            , XmlElement documentoAppend
            , Element elemento
            , JToken token
            , JToken valorToken
            , string caminhoItem)
        {
            int contador = 0;
            List<Element> elementosContratoArray = elemento.No;
            var tokens = token.SelectTokens(caminhoItem);

            foreach (var tokenItemArray in valorToken)
            {
                XmlElement itemArray = document.CreateElement($"{elemento.Nome}");

                foreach (var itemContratoArray in elementosContratoArray)
                {
                    string subCaminhoArray = string.Empty;
                    this.ProcessarListaElement(schema, document, itemArray, itemContratoArray, tokenItemArray, subCaminhoArray);
                }

                documentoAppend.AppendChild(itemArray);

                contador += 1;
            }
        }

        #endregion

        #region PROCESSAR ITEM SIMPLES

        internal void ProcessarListaElementSimples(
              Schema schema
            , XmlDocument document
            , XmlElement item
            , XmlElement documentoAppend
            , Element elemento
            , JToken token
            , string caminhoItem)
        {
            // ADICIONAR ITEM AO DOM DO XML
            documentoAppend.AppendChild(item);

            foreach (var no in elemento.No)
            {
                this.ProcessarListaElement(schema, document, item, no, token, caminhoItem);
            }

            if(item.IsEmpty)
            {
                documentoAppend.RemoveChild(item);
            }
        }

        #endregion

        #region PROCESSAR VALOR DO TIPO JSON

        private void CarregarPropriedadeEValidacoes(XmlElement item, JToken valorToken, Element elemento, string caminhoItem)
        {
            if (valorToken is null)
                return;


            switch (valorToken.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Object:
                    break;
                case JTokenType.Array:

                    if (elemento.Processador.TiposProcessador != TiposProcessadores.OBJETO_ARRAY)
                        throw new ArgumentException($"Tipo não compativel entre elementos: {caminhoItem}");

                    break;
                case JTokenType.Constructor:
                case JTokenType.Property:
                case JTokenType.Comment:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:

                    //if (elemento.Processador.TiposProcessador != TiposProcessadores.STRING)
                    //    throw new ArgumentException($"Tipo não compativel entre elementos: {caminhoItem}");

                    var valor = ProcessadoresHelper.CarregarValorFormatado(elemento, valorToken.ToString());
                    item.InnerText = valor.ToString();

                    break;

                case JTokenType.Null:

                    item.InnerText = null;

                    break;
                case JTokenType.Undefined:
                    break;
            }

          //  item.InnerText = caminhoItem;
        }

        #endregion


        #endregion


















        public List<Element> ConverterContrato(Schema schema)
        {
            string json = schema.Resultado;
            return this.ProcessarJson(schema, json);
        }

        public string RecuperarPrefixo(Schema schema)
        {
            return string.IsNullOrEmpty(schema.Servico.Prefixo) ? string.Empty : $"{schema.Servico.Prefixo}:";
        }

        #region PROCESSAR ELEMENTO JSON

        private List<Element> ProcessarJson(Schema schema, string json)
        {
            List<Element> elements = new List<Element>();
            XmlNode? nodeRetorno = this.RecuperarNodePrinciaplRetorno(schema);
            JObject obj = JObject.Parse(json);

            //TODO: VALIDAR DADO RETORNO
            // nodeRetorno

            Element element = this.ProcessarElemento(schema, nodeRetorno);
            elements.Add(element);

            return elements;
        }

        private XmlNode? RecuperarNodePrinciaplRetorno(Schema schema)
        {
            // Criar o namespace manager
            XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
            ns.AddNamespace(schema.Servico.Prefixo, "http://www.w3.org/2001/XMLSchema");

            string path = $"//{schema.Servico.Prefixo}:element[@name='{schema.NomeServico}Response']";
            var node = schema.Contrato.SelectNodes(path, ns);

            // CASO NULO - PEGAR EM ITENS IMPORTADOS
            if (node is null || node.Count < 1)
            {
                node = base.RecuperarElementoImportacaoServico(schema, path);
            }

            if (node is null || node.Count < 1) return null;
            var noResponse = node[0];

            return noResponse;
        }

        internal Element ProcessarElementoJson(KeyValuePair<string, JToken> item, XmlNode noResponse, XmlNamespaceManager ns)
        {
            Element element = new Element();

            string tipagemContrato = this.ProcessarXmlTipoPropriedade(item, noResponse, ns);
            TiposProcessadores tipoProcessador = this.RecuperarTipoProcessador(item.Value);

            //TODO: VALIDACAO
            //this.ValidarTipoContrato(item, tipoProcessador, tipagemContrato);

            element.Nome = this.TratarNomePropriedade(item.Key);
            element.Valor = item.Value.ToString();
            element.Tipo = this.RecuperarTipoElemento(item.Value);
            element.Processador.TiposProcessador = tipoProcessador;

            // TODO: ITENS FILHOS
            //if(element.Tipo == XmlNodeType.Element)
            //{
            //    var token = item.Value;
            //    element.No.AddRange();
            //}

            return element;
        }

        private string TratarNomePropriedade(string key)
        {
            if (key.Length == 0) return string.Empty;

            if (key.Length == 1 && char.IsLower(key[0]))
            {
                return char.ToUpper(key[0]).ToString();
            }
            else if (char.IsLower(key[0]))
            {
                char p1 = char.ToUpper(key[0]);
                return char.ToUpper(p1) + key.Substring(1);
            }

            return key;
        }

        #endregion

        #region PROCESSAR XML

        private string ProcessarXmlTipoPropriedade(KeyValuePair<string, JToken> item, XmlNode noResponse, XmlNamespaceManager ns)
        {
            string key = item.Key;
            var propXml = noResponse.SelectNodes($"//xs:element[{this.TratarCaseSensitive("@name")}='{key.ToLower()}']", ns);

            if (propXml is null || propXml.Count == 0)
            {
                return string.Empty;
                // throw new ArgumentException($"Propriedade não localizada no contrato '{key}'");
            }

            var prop = propXml.Item(0);

            if (prop is null || prop.Attributes is null || prop.Attributes.Count == 0)
            {
                return string.Empty;
               // throw new ArgumentException($"Atributos da propriedade não localizada no contrato '{key}'");
            }

            var atributo = prop.Attributes.Cast<XmlAttribute>().FirstOrDefault(e => e.Name == "type");
            
            if (atributo is null) return string.Empty;

            string typeProp = atributo.Value;
            return typeProp.Contains(":") ? typeProp.Split(":")[1] : typeProp;
        }

        #endregion

        #region VALIDADOR TIPO CONTRATO

        private void ValidarTipoContrato(KeyValuePair<string, JToken> item, TiposProcessadores tipoProcessador, string tipagemContrato)
        {
            if (item.Value.Type == JTokenType.Null)
                return;

            // TODO: VALIDAR TIPAGEM PARA OUTROS ITENS
            if (!tipoProcessador.ToString().ToLower().Equals(tipagemContrato.ToLower()))
                throw new ArgumentException($"Tipo '{item.Key}' com valor não suportado: {tipagemContrato}");
        }

        #endregion

        #region TIPO ELEMENTO XML

        private XmlNodeType RecuperarTipoElemento(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.Array:
                case JTokenType.Object:
                    return XmlNodeType.Element;

            }

            return XmlNodeType.Text;
        }

        #endregion

        #region TIPOS PROCESSADORES

        private TiposProcessadores RecuperarTipoProcessador(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Object:
                    break;
                case JTokenType.Array:
                    break;
                case JTokenType.Constructor:
                    break;
                case JTokenType.Property:
                    break;
                case JTokenType.Comment:
                    break;
                case JTokenType.Integer:
                    break;
                case JTokenType.Float:
                    break;
                case JTokenType.String:
                    return TiposProcessadores.STRING;

                case JTokenType.Boolean:
                    break;
                case JTokenType.Null:
                    break;
                case JTokenType.Undefined:
                    break;
                case JTokenType.Date:
                    break;
                case JTokenType.Raw:
                    break;
                case JTokenType.Bytes:
                    break;
                case JTokenType.Guid:
                    break;
                case JTokenType.Uri:
                    break;
                case JTokenType.TimeSpan:
                    break;
            }

            return TiposProcessadores.DEFAULT;
        }

        #endregion


        #region PROCESSAR ELEMENTOS PARA XML

        internal XmlDocument ProcessarElementos(Schema schema, List<Element> elementos)
        {
            XmlDocument soapDoc = new XmlDocument();
            string prefixo = this.RecuperarPrefixo(schema);

            // Criar Envelope com namespace SOAP
            XmlElement envelope = soapDoc.CreateElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            soapDoc.AppendChild(envelope);

            // Criar Body
            XmlElement body = soapDoc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            envelope.AppendChild(body);

            // Criar elemento raiz
            XmlElement response = soapDoc.CreateElement(schema.Servico.Prefixo, $"{schema.NomeServico}Response", schema.Servico.UrlHost);
            body.AppendChild(response);

            for (int i = 0; i < elementos.Count; i += 1)
            {
                Element elemento = elementos[i];
                XmlElement item = ConverterXml(elemento, soapDoc, schema);

                response.AppendChild(item);
            }
            return soapDoc;
        }

        internal XmlElement ConverterXml(Element elemento, XmlDocument document, Schema schema)
        {
            XmlElement item = document.CreateElement(schema.Servico.Prefixo, $"{elemento.Nome}", schema.Servico.UrlHost);
            // item.SetAttribute(this.Nome, this.Valor);
            item.InnerText = elemento.Valor;

            return item;
        }

        #endregion
    }
}
