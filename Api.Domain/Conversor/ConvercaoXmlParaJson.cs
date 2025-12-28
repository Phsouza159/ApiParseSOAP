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

            //TESTE
            //return JsonConvert.SerializeObject(new { elementosCorpo, lista});

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
            schema.IsElementoEntrada = true;

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

            schema.IsElementoEntrada = false;

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
                if (elemento.Tipo == XmlNodeType.Element)
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
