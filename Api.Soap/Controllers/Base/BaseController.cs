
using Api.Domain.Configuracao;
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.Interfaces;
using Api.Domain.Exceptions;
using Api.Domain.Enum;

namespace ApiParseSOAP.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        public IConfiguration Config { get; }

        protected BaseController(IConfiguration config)
        {
            Config = config;
        }

        #region RECUPERAR CORPO CHAMADA

        internal async Task<string> RecuperarCorpoChamada()
        {
            using var reader = new StreamReader(Request.Body);
            return await reader.ReadToEndAsync();
        }

        #endregion

        #region TRATAR ERRO

        internal async Task<IActionResult> TratamentoErroFatal(Exception ex, IServicoLog servicoLog)
        {
            string servico = this.Request.Path;
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            string templete = await this.RecuperarArquvioTemplete(servicoConfiguracao, TipoArquivoTemplete.ERRO);

            templete = templete.Replace("{{MENSAGEM}}", ex.Message);
            this.Response.StatusCode = 500;

            servicoLog.CriarLog(servico, ex.RecuperarTraceErroTratado(), TipoLog.TRACE_ERRO);
            servicoLog.CriarLog(servico, templete, TipoLog.RETORNO_XML);
            await servicoLog.Save();

            return Content(templete, "text/xml");
        }

        #endregion

        #region PROCESSAR RESPOSTAS

        internal async Task<IActionResult> ProcessarResposta(IActionResult actionresult, IServicoLog servicoLog)
        {
            if (actionresult is NotFoundResult)
                servicoLog.CriarLog(this.Request.Path, "SERVICO NAO LOCALIZADO", TipoLog.INFO);

            await servicoLog.Save();
            return actionresult;
        }

        #endregion

        #region CARREGAR ARQUIVOS TEMPLETES PADRAO

        private async Task<string> RecuperarArquvioTemplete(Servicos? servico, TipoArquivoTemplete tipoArquivo)
        {
            string path;

            // TEMPLETE SERVICO
            if (servico != null)
            {
                path = Path.Combine(servico.PastaRaizServico, $"TEMPLETE_{(short)tipoArquivo}.txt");

                if (System.IO.File.Exists(path))
                    return await System.IO.File.ReadAllTextAsync(path);
            }

            // TEMPLETE DEFAULT
            path = Path.Combine(this.Config.GetPathServicos(), "Templetes", $"TEMPLETE_{(short)tipoArquivo}.txt");

            if (System.IO.File.Exists(path))
                return await System.IO.File.ReadAllTextAsync(path);

            return "{{MENSAGEM}}";
        }

        #endregion
    }
}
