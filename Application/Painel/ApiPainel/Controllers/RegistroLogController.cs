using Api.Domain.Api.Domain;
using Api.Domain.Interfaces.Facede;
using Api.Domain.ObjectValues;
using ApiPainel.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApiPainel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroLogController : Controller
    {
        public RegistroLogController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(
                 [FromQuery] DateTime? dataInicio
               , [FromQuery] DateTime? dataFim
               , [FromServices] IRegistroLogFacede registroLogFacede
            )
        {
            try
            {

                if (dataInicio.HasValue && dataFim.HasValue)
                {
                    var p = new ParametrosPesquisaDTO(dataInicio, dataFim);
                    return base.Ok(registroLogFacede.RecuperarRegistroLog(p.DataInicio, p.DataFim));
                }

                return BadRequest(new ResponseData<string>() { Mensagem = "Parametros de pesquisa inválido"});
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
