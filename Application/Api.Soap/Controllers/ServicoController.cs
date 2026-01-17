using Api.Domain.Api;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Exceptions;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using ApiParseSOAP.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicoController : BaseController
    {
        public ServicoController(IConfiguration config)
            : base(config)
        {
        }


        #region SERVICO WSDL - GET

        [HttpGet]
        [Route("{servico}")]
        public async Task<IActionResult> Get(
              [FromRoute] string servico
            , [FromQuery] string? wsdl
            , [FromHeader] string? queryInfo
            , [FromServices] IServicoLog servicoLog)
        {
            try
            {
                if (!string.IsNullOrEmpty(queryInfo) && queryInfo == "INFO")
                    return Ok(ServicoArquivosWsdl.Configuracacoes);

                Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

                if (servicoConfiguracao != null)
                {
                    string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servico);

                    if (servicoConfiguracao.ConteudoArquivos.TryGetValue(nomeTratado, out byte[]? value))
                    {
                        string data = string.Format("Serviço WSDL/XSD recuperado: {0}", Encoding.UTF8.GetString(value));
                        servicoLog.CriarLog(servico, data, TipoLog.INFO);
                        return await base.ProcessarResposta(File(value, "text/xml"), servicoLog);
                    }
                }

                return await base.ProcessarResposta(NotFound(), servicoLog);
            }
            catch (Exception ex)
            {
                return await base.TratamentoErro(ex, servicoLog);
            }
        }

        #endregion

        #region SERVICO - POST

        [HttpPost]
        [Route("{servico}")]
        public async Task<IActionResult> Post(
                  [FromRoute]  string servico
                , [FromHeader] string? queryParametro
                , [FromServices] IProcessarChamadaSoapFacede processardorChamadaFacede
                , [FromServices] IServicoLog servicoLog
            )
        {
            try
            {
                string xmlConteudo = await base.RecuperarCorpoChamada();

                servicoLog.IsDebug = queryParametro == "DEBUG";
                servicoLog.CriarLog(servico, xmlConteudo, TipoLog.ENTRADA_XML);

                string autenticacao = this.Request.Headers.Authorization.ToString() ?? string.Empty;
                using Schema schema = processardorChamadaFacede.CarregarDadosChamadaSoap(servico, xmlConteudo, autenticacao);

                // DEFAULT 404
                if (schema.IsVazio)
                    return await base.ProcessarResposta(NotFound(), servicoLog);

                var processamento = await processardorChamadaFacede.ProcessarFluxoSoap(schema, servicoLog);
                return await base.ProcessarResposta(Content(processamento.Conteudo, processamento.TipoConteudo), servicoLog);
            }
            catch (Exception ex)
            {
                return await base.TratamentoErro(ex, servicoLog);
            }
        }

        #endregion
    }
}
