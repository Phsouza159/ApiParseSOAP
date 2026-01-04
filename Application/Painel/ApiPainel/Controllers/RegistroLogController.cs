using Microsoft.AspNetCore.Mvc;

namespace ApiPainel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistroLogController : Controller
    {
        public RegistroLogController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(
                 [FromForm] DateTime dataInicio
               , [FromForm] DateTime dataFim
               , [FromServices] IConfiguration configuration
            )
        {
            return Ok();
        }
    }
}
