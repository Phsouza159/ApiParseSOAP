using Api.Domain.Api;
using Api.Domain.Enum;
using Api.Domain.Helper;
using Api.Domain.Interfaces;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;

namespace Api.Domain.Conversor
{
    public class ConvercaoXmlParaJson : Base.Conversor, IConvercaoXmlParaJson
    {
        public ConvercaoXmlParaJson()
        {
        }

        public string ConverterParaJson(Schema schema)
        {
            //RECUPERAR LISTA COMPLETA - CONTRATO + VALORES ENVELOPE
            List<Elemento> lista = this.ConverterContrato(schema);

            // [DEBUG] - MAPA XML
            //return JsonConvert.SerializeObject(lista);

            // CARREGAR MENSAGENS DO CONTRATO
            lista.ForEach(e => this.Notificacoes.AdicionarMensagem(e.RecuperarNotificacoesItensFilhos()));

            // RECUPERAR LISTA TRATADA - APENAS ELEMENTOS 
            List<Elemento> elementosCorpo = this.TratarLista(lista);

            // CONVERTER LIST<ELEMENTOS> PARA OBJETO JSON
            JsonObject data = this.ProcessarElementos(elementosCorpo, schema);

            base.ValidarNotifcacoes();

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
        public List<Elemento> ConverterContrato(Schema schema)
        {
            this.Encapsulamento = schema.Servico.ArquivosWsdl.Count == 1 ? TipoEncapsulamento.ENCAPSULAMENTO : TipoEncapsulamento.MULTIPLO;

            List<Elemento> lista = new();
            // TODO: VALIDAR xs e xsd e CONFIGURAR PREFIXO
            string prefixoBusca = "xsd";

            foreach (var item in schema.XmlNodes)
            {
                if (item != null && item.NodeType != XmlNodeType.Comment)
                {
                    Elemento element = this.ProcessarElemento(schema, item, prefixoBusca);
                    element.Processador.IsPropriedade = true;
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
        internal JsonObject ProcessarElementos(List<Elemento> elementos, Schema schema)
        {
            JsonObject json = new JsonObject();

            for (int i = 0; i < elementos.Count; i += 1)
            {
                Elemento elemento = elementos[i];
                if (elemento.Tipo == XmlNodeType.Element)
                    this.ProcessarElementoParaJson(json, elemento, schema);

                //Carregar Notificacoes
                this.Notificacoes.AdicionarMensagem(elemento.RecuperarNotificacoesItensFilhos());
            }

            return json;
        }

        /// <summary>
        /// CRIAR NO ELEMENTO DENTRO DO OBJETO JSON
        /// </summary>
        internal void ProcessarElementoParaJson(JsonObject json, Elemento elemento, Schema schema)
        {
            if (elemento.Valor is null && schema.Servico.IgnorarNulo)
                return;

            var objet = this.ConverterElementoParaObjeto(elemento, schema);

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
        internal object? ConverterElementoParaObjeto(Elemento elemento, Schema schema)
        {
            //string defaultNull = "null";

            switch (elemento.Processador.TiposProcessador)
            {
                case TiposProcessadores.OBJETO:
                case TiposProcessadores.OBJETO_IMPORTADO:
                    return this.ConverterElementoComplexoParaObjeto(elemento, schema);

                default:
                    return elemento.CarregarValorFormatado(elemento.Valor);
            }
        }

        /// <summary>
        /// CONVERTER ELEMENTO PARA OBJETOS COMPLEXOS
        /// </summary>
        internal object ConverterElementoComplexoParaObjeto(Elemento elemento, Schema schema)
        {
            JsonObject json = new JsonObject();

            foreach (var no in elemento.ElementosFilhos)
            {
                if (no.Valor is null && schema.Servico.IgnorarNulo)
                    continue;

                var valor = this.ConverterElementoParaObjeto(no, schema);
                json[no.Nome] = JsonValue.Create(valor);

            }

            return json;
        }

        #endregion
    }
}
