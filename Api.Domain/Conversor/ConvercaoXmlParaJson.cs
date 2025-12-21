using Api.Domain.Configuracao;
using Api.Domain.Enum;
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
    public class ConvercaoXmlParaJson : Base.Conversor, IConvercaoXmlParaJson
    {
        public string ConverterParaJson(Schema schema)
        {
            // RECUPERAR LISTA COMPLETA - CONTRATO + VALORES ENVELOPE
            List<Element> lista = this.ConverterContrato(schema);

            //TODO - VALIDAR
            // CRIAR FUNCAO PARA VALIDAR ELEMENTOS

            // RECUPERAR LISTA TRATADA - APENAS ELEMENTOS 
            List<Element> elementosCorpo = this.TratarLista(lista);

            // CONVERTER LIST<ELEMENTOS> PARA OBJETO JSON
            JsonObject data = this.ProcessarElementos(elementosCorpo);

            // RECUPERAR JSON EM FORMA DE STRING
            return data.ToJsonString();
        }

        #region CONVERT DADOS

        /// <summary>
        /// CARREGAR DADOS DO ESQUEMA XML E TRANFORMAR EM UMA LISTA DO TIPO <paramref name="Element"/>
        /// </summary>
        /// <param name="schema">
        /// Esquema XML
        /// </param>
        /// <returns>
        /// <list type="Element">Dados de NO do XML do Contrato + VALOR DO CORPO DO ENVELOPE DE ENVIO</list>
        /// </returns>
        public List<Element> ConverterContrato(Schema schema)
        {
            List<Element> lista = new();
            // TODO: VALIDAR xs e xsd e CONFIGURAR PREFIXO
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

        #endregion

        #region PROCESSAR ELEMENTOS PARA JSON

        /// <summary>
        /// PROCESSAR ELEMENTOS PARA OBJETO JSON
        /// </summary>
        internal JsonObject ProcessarElementos(List<Element> elementos)
        {
            JsonObject json = new JsonObject();

            for (int i = 0; i < elementos.Count; i += 1)
            {
                Element elemento = elementos[i];
                this.ProcessarElementoParaJson(json, elemento);
            }

            return json;
        }

        /// <summary>
        /// CRIAR NO ELEMENTO DENTRO DO OBJETO JSON
        /// </summary>
        internal void ProcessarElementoParaJson(JsonObject json, Element elemento)
        {
            var objet = this.ConverterElementoParaObjeto(elemento);

            if (objet is JsonValue data)
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

        /// <summary>
        /// CONVERTER ELEMENTO PARA OBJETO
        /// </summary>
        internal object ConverterElementoParaObjeto(Element elemento)
        {
            string defaultNull = "null";

            switch (elemento.Processador.TiposProcessador)
            {
                case TiposProcessadores.OBJETO:
                case TiposProcessadores.OBJETO_IMPORTADO:
                    return this.ConverterElementoComplexoParaObjeto(elemento);

                default:
                    return elemento.CarregarValorFormatado(elemento.Valor);
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

        /// <summary>
        /// CONVERTER ELEMENTO PARA OBJETOS COMPLEXOS
        /// </summary>
        internal object ConverterElementoComplexoParaObjeto(Element elemento)
        {
            JsonObject json = new JsonObject();

            foreach (var no in elemento.No)
            {
                json[no.Nome] = JsonValue.Create(this.ConverterElementoParaObjeto(no));
            }

            return json;
        }

        #endregion
    }
}
