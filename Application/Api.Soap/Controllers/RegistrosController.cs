
using Api.Domain.Api.Ressources;
using Api.Domain.Extensions;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrosController : Base.BaseController
    {
        public RegistrosController(IConfiguration config) : base(config)
        {
        }

        [HttpGet]
        public async Task<IActionResult> RecuperarLista([FromServices] IServicoLog servicoLog)
        {
            try
            {
                string source = ArquivosRessource.DataListaServico;
                string host = this.Config.RecuperarHostServico();
                string listaServicos = string.Join("", ServicoArquivosWsdl.Configuracacoes
                        .Select(e => $"<li class=\"list-group-item\"><a href=\"{host}/{e.Nome}?wsdl\" target=\"_blank\">{e.Nome}</a></li>"));

                source = source.Replace("{LISTA_SERVICOS}", listaServicos);
                return Content(source, "text/html");
            }
            catch (Exception ex)
            {
                return await base.TratamentoErro(ex, servicoLog);
            }
        }
    }
}
