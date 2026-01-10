using Api.Domain.Api.Domain;
using Api.Domain.Interfaces.Facede;
using Api.Domain.ObjectValues;
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
                 [FromForm] DateTime dataInicio
               , [FromForm] DateTime dataFim
               , [FromServices] IRegistroLogFacede registroLogFacede
            )
        {
            try
            {
                return base.Ok(registroLogFacede.RecuperarRegistroLog(dataInicio, dataFim));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
