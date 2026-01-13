using Api.Domain.Api;
using Api.Domain.Api.Domain.Autenticacao;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using Api.Domain.Services;
using System.Text;

namespace Api.Application.Facede
{
    public class ProcessarChamadaSoapFacede : IProcessarChamadaSoapFacede
    {
        public ProcessarChamadaSoapFacede(
                  IConvercaoXmlParaJson conversaoJson
                , IConvercaoJsonParaXml conversaoXml
                , IServicoIntegracaobFacede servicoIntegracaoFacede
            )
        {
            ConversaoJson = conversaoJson;
            ConversaoXml = conversaoXml;
            ServicoIntegracaoFacede = servicoIntegracaoFacede;
        }

        public IConvercaoXmlParaJson ConversaoJson { get; }

        public IConvercaoJsonParaXml ConversaoXml { get; }

        public IServicoIntegracaobFacede ServicoIntegracaoFacede { get; }

        #region CRIAR DADOS SCHEMA XML

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
            var contrato = schema.RecuperarContrato();

            if(contrato != null && !string.IsNullOrEmpty(autenticacao) && !string.IsNullOrEmpty(contrato.TipoAutenticacao))
            {
                contrato.Autenticacao = contrato.TipoAutenticacao switch
                {
                    "REDIRECIONAR_AUTENTICACAO" => new RedirecionamentoAutenticacao(autenticacao),
                    // DEFAULT
                    _ => throw new ArgumentException($"Tipo autenticação não suportado: {contrato.Autenticacao}"),
                };
            }
        }

        #endregion

        #region PROCESSAR FLUXO 

        public async Task<ProcessamentoFluxoSoap> ProcessarFluxoSoap(Schema schema, IServicoLog servicoLog)
        {
            ProcessamentoFluxoSoap processamento = new ProcessamentoFluxoSoap();

            if (servicoLog.IsDebug)
                return this.ProcessarFluxoDebug(schema, processamento, servicoLog);

            var envelope = await this.ServicoIntegracaoFacede.EnviarProcessamento(schema, servicoLog);

            schema.Resultado = envelope.ConteudoRetorno;

            if (string.IsNullOrEmpty(schema.Resultado))
                throw new ArgumentException("Resposta vazia no tratamento de fluxo.");

            string xmlResposta = this.ConversaoXml.ConverterParaXml(schema);
            servicoLog.CriarLog(schema.Servico.Nome, xmlResposta, TipoLog.RETORNO_XML);

            processamento.Conteudo = xmlResposta;
            processamento.TipoConteudo = "text/xml";

            return processamento;
        }

        private ProcessamentoFluxoSoap ProcessarFluxoDebug(Schema schema, ProcessamentoFluxoSoap processamento, IServicoLog servicoLog)
        {
            var json = this.ConversaoJson.ConverterParaJson(schema);
            servicoLog.CriarLog(schema.Servico.Nome, json, TipoLog.DEBUG);

            processamento.Conteudo = json;
            processamento.TipoConteudo = "application/json";

            return processamento;
        }

        #endregion
    }
}
