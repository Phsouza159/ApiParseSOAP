
using Api.Domain.Configuracao;
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.Interfaces;
using Api.Domain.Exceptions;
using Api.Domain.Enum;
using Api.Domain;
using Api.Domain.Api.Domain.Autenticacao;

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

        #region TRATAR ERRO FATAL

        internal async Task<IActionResult> TratamentoErro(Exception ex, IServicoLog servicoLog)
        {
            if(ex is AutorizacaoException autorizacaoException)
                return await this.TratamentoSemAutorizacao(autorizacaoException, servicoLog);

            return await this.TratamentoSemAutorizacao(ex, servicoLog, TipoArquivoTemplete.ERRO
                , TipoLog.TRACE_ERRO, StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region TRATAR SEM AUTORIZACAO

        internal async Task<IActionResult> TratamentoSemAutorizacao(AutorizacaoException ex, IServicoLog servicoLog)
        {
            return await this.TratamentoSemAutorizacao(ex, servicoLog, TipoArquivoTemplete.SEM_AUTORIZACAO
                , TipoLog.TRACE_ERRO, StatusCodes.Status401Unauthorized);
        }

        #endregion

        internal async Task<IActionResult> TratamentoSemAutorizacao(
              Exception ex
            , IServicoLog servicoLog
            , TipoArquivoTemplete tipoArquivo
            , TipoLog tipoLog
            , int statusCode
            )
        {
            string servico = this.Request.Path;
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            string templete = await this.RecuperarArquvioTemplete(servicoConfiguracao, tipoArquivo);

            templete = templete.Replace("{{MENSAGEM}}", ex.Message);
            this.Response.StatusCode = statusCode;

            servicoLog.CriarLog(servico, ex.RecuperarTraceErroTratado(), tipoLog);
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

        #region CARREGAR DADOS AUTENTICACAO
        internal void CarregarDadosAutenticacao(Schema schema, IHeaderDictionary headers)
        {
            switch (schema.Servico.Contratos[0].Autenticacao)
            {
                default:
                    break;
            }


            schema.Autenticacao = new RedirecionamentoAutenticacao(headers["Authorization"]);
        }

        #endregion

    }
}
