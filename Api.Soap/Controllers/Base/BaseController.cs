
using Api.Domain.Configuracao;
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ApiParseSOAP.Controllers.Base
{
    public abstract class BaseController : Controller
    {

        public IConfiguration Config { get; }

        protected BaseController(IConfiguration config)
        {
            Config = config;
        }

        internal string RecuperarCorpoChamada()
        {
            using var reader = new StreamReader(Request.Body);
            return reader.ReadToEnd();
        }

        internal async Task<IActionResult> TratamentoErroFatal(Exception ex)
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
            return Content(templete, "text/xml");
        }
    }
}
