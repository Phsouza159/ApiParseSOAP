using Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using ApiParseSOAP.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("{servico}")]
    public class ServicoSoapController : BaseController
    {
        public ServicoSoapController(IConfiguration config)
            : base(config)
        {
        }


        [HttpGet]
        public IActionResult Get(string servico, [FromQuery] string? wsdl)
        {
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if (servicoConfiguracao != null)
            {
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servico);

                if (servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
                {
                    byte[] data = servicoConfiguracao.ConteudoArquivos[nomeTratado];
                    return File(data, "text/xml");
                }
            }

            return NotFound(); 
        }

        [HttpPost]
        public async Task<IActionResult> Post(
                  [FromServices] IProcessarChamadaSoapFacede processardorChamadaFacede
                , [FromServices] IServicoWebFacede servicoWebFacede
                , [FromServices] IConvercaoXmlParaJson conversaoJson
                , [FromServices] IConvercaoJsonParaXml conversaoXml
                , [FromServices] IServicoLog servicoLog
                , [FromHeader] string? queryParametro
            )
        {
            try
            {
                string servico = this.Request.Path;
                string xmlConteudo = await base.RecuperarCorpoChamada();

                servicoLog.CriarLog(servico, xmlConteudo, TipoLog.ENTRADA_XML);

                string autenticacao = this.Request.Headers.Authorization.ToString() ?? string.Empty;
                using Schema schema = processardorChamadaFacede.CarregarDadosChamadaSoap(servico, xmlConteudo, autenticacao);

                // DEFAULT 404
                if (schema.IsVazio)
                    return await base.ProcessarResposta(NotFound(), servicoLog);
              
                if (queryParametro == "DEBUG")
                {
                    // DEBUG 
                    var json = conversaoJson.ConverterParaJson(schema);
                    servicoLog.CriarLog(servico, json, TipoLog.DEBUG);
                    return await base.ProcessarResposta(Content(json, "application/json"), servicoLog);
                }

                var envelope = await servicoWebFacede.EnviarProcessamento(schema, servicoLog);
                schema.Resultado = envelope.ConteudoRetorno;

                string xmlResposta = conversaoXml.ConverterParaXml(schema);

                servicoLog.CriarLog(servico, xmlResposta, TipoLog.RETORNO_XML);
                return await base.ProcessarResposta(Content(xmlResposta, "text/xml"), servicoLog);
            }
            catch (AutorizacaoException ex)
            {
                return await base.TratamentoSemAutorizacao(ex, servicoLog);
            }
            catch (Exception ex)
            {
                return await base.TratamentoErro(ex, servicoLog);
            }
        }
    }
}
