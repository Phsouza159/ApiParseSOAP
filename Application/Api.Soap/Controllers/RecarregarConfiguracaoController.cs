using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using ApiParseSOAP.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecarregarConfiguracaoController : BaseController
    {
        public RecarregarConfiguracaoController(IConfiguration config)
            : base(config)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(
                [FromServices] IServicoLog servicoLog
            )
        {
            try
            {
                ServicoArquivosWsdl.RecarregarArquivosConfiguracao(servicoLog);
                servicoLog.CriarLog("Recarregamento de configuração", "Recarregamento com sucesso.", TipoLog.INFO);

                return await this.ProcessarResposta(Ok(), servicoLog);
            }
            catch (Exception ex)
            {
               servicoLog.CriarLog(ex, "Erro ao recarregar arquivos de configuração");
               return await this.ProcessarResposta(this.StatusCode(StatusCodes.Status500InternalServerError), servicoLog);
            }
        }
    }
}
