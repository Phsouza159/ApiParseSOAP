
using Api.Domain.Configuracao;
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.Interfaces;
using Api.Domain.Exceptions;

namespace ApiParseSOAP.Controllers.Base
{
    public abstract class BaseController : Controller
    {

        public IConfiguration Config { get; }

        protected BaseController(IConfiguration config)
        {
            Config = config;
        }

        internal async Task<string> RecuperarCorpoChamada()
        {
            using var reader = new StreamReader(Request.Body);
            return await reader.ReadToEndAsync();
        }

        internal async Task<IActionResult> TratamentoErroFatal(Exception ex, IServicoLog servicoLog)
        {
            string templete = string.Empty;
            string servico = this.Request.Path;
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if(servicoConfiguracao != null)
            {
                templete = await servicoConfiguracao.RecuperarArquvioTempleteErro();
            }

            if(string.IsNullOrEmpty(templete))
            {
                string path = Path.Combine(this.Config.GetPathServicos(), "Templetes", "ERRO_500.txt");
                templete = await System.IO.File.ReadAllTextAsync(path);
            }

            templete = templete.Replace("{{MENSAGEM}}", ex.Message);
            this.Response.StatusCode = 500;

            servicoLog.CriarLog(servico, ex.RecuperarTraceErroTratado(), Api.Domain.Enum.TipoLog.TRACE_ERRO);
            servicoLog.CriarLog(servico, templete, Api.Domain.Enum.TipoLog.RETORNO_XML);
            await servicoLog.Save();

            return Content(templete, "text/xml");
        }

        internal async Task<IActionResult> ProcessarResposta(IActionResult actionresult, IServicoLog servicoLog)
        {
            await servicoLog.Save();
            return actionresult;
        }
    }
}
