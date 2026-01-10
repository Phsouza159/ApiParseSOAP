using Api.Domain.Api;
using Api.Domain.Api.Domain.Autenticacao;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using Api.Domain.Services;
using System.Text;
using System.Web.Helpers;

namespace Api.Application.Facede
{
    public class ProcessarChamadaSoapFacede : IProcessarChamadaSoapFacede
    {
        public ProcessarChamadaSoapFacede(
                  IServicoIntegracao servicoWeb
                , IConvercaoXmlParaJson conversaoJson
                , IConvercaoJsonParaXml conversaoXml
                , IServicoIntegracaobFacede servicoWebFacede
            )
        {
            ServicoWeb = servicoWeb;
            ConversaoJson = conversaoJson;
            ConversaoXml = conversaoXml;
            ServicoWebFacede = servicoWebFacede;
        }

        public IServicoIntegracao ServicoWeb { get; }
        public IConvercaoXmlParaJson ConversaoJson { get; }
        public IConvercaoJsonParaXml ConversaoXml { get; }
        public IServicoIntegracaobFacede ServicoWebFacede { get; }

        #region CRIAR DADOS SCHEMA

        public Schema CarregarDadosChamadaSoap(string servico, string xmlConteudo, string autenticacao)
        {
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if (servicoConfiguracao != null)
            {
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servicoConfiguracao.Nome);

                if (servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
                {
                    byte[] data = servicoConfiguracao.ConteudoArquivos[nomeTratado];
                    string xmlContrato = Encoding.UTF8.GetString(data);

                    var document = ServicoArquivosWsdl.TransformarXml(xmlContrato);
                    var conteudo = ServicoArquivosWsdl.TransformarXml(xmlConteudo);

                    var schema = ServicoArquivosWsdl.CriarSchemaXML(document, conteudo);

                    // VALIR SE SERVICO ESTA CONFIGURADO
                    if (!servicoConfiguracao.Contratos.Any(e => e.Servico.ToLower().Equals(schema.NomeServico.ToLower())))
                        throw new ArgumentException($"Serviço não registrado: '{schema.NomeServico}'");

                    schema.Servico = servicoConfiguracao;
                    
                    this.CarregarDadosAutenticacao(schema, autenticacao);

                    return schema;
                }
            }

            return new Schema() { IsVazio = true };
        }

        private void CarregarDadosAutenticacao(Schema schema, string autenticacao)
        {
            var contrato = schema.GetContrato();

            if(contrato != null && !string.IsNullOrEmpty(autenticacao) && !string.IsNullOrEmpty(contrato.Autenticacao))
            {
                schema.Autenticacao = contrato.Autenticacao switch
                {
                    "REDIRECIONAR_AUTENTICACAO" => new RedirecionamentoAutenticacao(autenticacao),
                    // DEFAULT
                    _ => throw new ArgumentException($"Tipo autenticação não suportado: {contrato.Autenticacao}"),
                };
            }
        }

        #endregion

        public async Task<ProcessamentoFluxoSoap> ProcessarFluxoSoap(Schema schema, IServicoLog servicoLog)
        {
            ProcessamentoFluxoSoap processamento = new ProcessamentoFluxoSoap();

            if(servicoLog.IsDebug)
            {
                var json = this.ConversaoJson.ConverterParaJson(schema);
                servicoLog.CriarLog(schema.Servico.Nome, json, TipoLog.DEBUG);
                processamento.Conteudo = json;
                processamento.TipoConteudo = "application/json";

                return processamento;
            }

            var envelope = await this.ServicoWebFacede.EnviarProcessamento(schema, servicoLog);
            schema.Resultado = envelope.ConteudoRetorno;

            if (string.IsNullOrEmpty(schema.Resultado))
                throw new ArgumentException("Resposta vazia no tratamento de fluxo.");

            string xmlResposta = this.ConversaoXml.ConverterParaXml(schema);
            servicoLog.CriarLog(schema.Servico.Nome, xmlResposta, TipoLog.RETORNO_XML);

            processamento.Conteudo = xmlResposta;
            processamento.TipoConteudo = "text/xml";

            //if (queryParametro == "DEBUG")
            //{
            //    // DEBUG 
            //    var json = conversaoJson.ConverterParaJson(schema);
            //    servicoLog.CriarLog(servico, json, TipoLog.DEBUG);
            //    return await base.ProcessarResposta(Content(json, "application/json"), servicoLog);
            //}

            //var envelope = await servicoWebFacede.EnviarProcessamento(schema, servicoLog);
            //schema.Resultado = envelope.ConteudoRetorno;

            //string xmlResposta = conversaoXml.ConverterParaXml(schema);

            //servicoLog.CriarLog(servico, xmlResposta, TipoLog.RETORNO_XML);
            //return await base.ProcessarResposta(Content(xmlResposta, "text/xml"), servicoLog);


            return processamento;
        }
    }
}
