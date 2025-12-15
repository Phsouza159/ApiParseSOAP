using Api.Domain.Conversor.Extensions;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Api.Domain.Conversor
{
    public class ConvercaoJsonParaXml : IConvercaoJsonParaXml
    {
        public List<Element> Converter(Schema schema)
        {
            string json = schema.Resultado;
            return this.ProcessarJson(schema, json);
        }

        #region PROCESSAR ELEMENTO JSON

        private List<Element> ProcessarJson(Schema schema, string json)
        {
            List<Element> elements = new List<Element>();

            // Criar o namespace manager
            XmlNamespaceManager ns = new XmlNamespaceManager(schema.Contrato.NameTable);
            ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

            JObject obj = JObject.Parse(json);
            var node = schema.Contrato.SelectNodes($"//xs:element[@name='{schema.NomeServico}Response']", ns);

            if (node is null || node.Count < 1) return elements;

            var noResponse = node[0];

            if (noResponse is null) return elements;

            foreach (var item in obj)
            {
                Element element = this.ProcessarElementoJson(item, noResponse, ns);
                elements.Add(element);
            }

            return elements;
        }

        internal Element ProcessarElementoJson(KeyValuePair<string, JToken> item, XmlNode noResponse, XmlNamespaceManager ns)
        {
            Element element = new Element();

            string tipagemContrato = this.ProcessarXmlTipoPropriedade(item, noResponse, ns);
            TiposProcessadores tipoProcessador = this.RecuperarTipoProcessador(item.Value);

            this.ValidarTipoContrato(item, tipoProcessador, tipagemContrato);

            element.Nome = item.Key;
            element.Valor = item.Value.ToString();
            element.Tipo = this.RecuperarTipoElemento(item.Value);
            element.Processador = tipoProcessador;

            // TODO: ITENS FILHOS
            //if(element.Tipo == XmlNodeType.Element)
            //{
            //    var token = item.Value;
            //    element.No.AddRange();
            //}

            return element;
        }

        #endregion

        #region PROCESSAR XML

        private string ProcessarXmlTipoPropriedade(KeyValuePair<string, JToken> item, XmlNode noResponse, XmlNamespaceManager ns)
        {
            string key = item.Key;
            var propXml = noResponse.SelectNodes($"//xs:element[{this.TratarCaseSensitive("@name")}='{key.ToLower()}']", ns);

            if (propXml is null || propXml.Count == 0) 
                throw new ArgumentException($"Propriedade não localizada no contrato '{key}'");

            var prop = propXml.Item(0);

            if (prop is null || prop.Attributes is null || prop.Attributes.Count == 0) 
                throw new ArgumentException($"Atributos da propriedade não localizada no contrato '{key}'");

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
    }
}
