using ApiParseSOAP.Domain;
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;
using System.Xml;
using Api.Domain.Configuracao;
using Api.Domain.Interfaces;
using Api.Domain.Facede;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ApiParseSOAP.Controllers.Base;
using Api.Domain.Enum;
using System.Web.Helpers;
using Api.Domain.Conversor;
using Api.Domain;

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("{servico}")]
    public class ServicoSoapController : BaseController
    {
        public ServicoSoapController(IConfiguration config)
            : base(config)
        {
        }


        [HttpGet]
        public IActionResult Get(string servico, [FromQuery] string? wsdl)
        {
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if (servicoConfiguracao != null)
            {
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servico);

                if (servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
                {
                    byte[] data = servicoConfiguracao.ConteudoArquivos[nomeTratado];
                    return File(data, "text/xml");
                }
            }

            return NotFound(); 
        }

        [HttpPost]
        public async Task<IActionResult> Post(
                  [FromHeader] string? queryParametro
                , [FromServices] IProcessarChamadaSoapFacede processardorChamada
                , [FromServices] IConvercaoXmlParaJson conversaoJson
                , [FromServices] IConvercaoJsonParaXml conversaoXml
                , [FromServices] IServicoLog servicoLog
            )
        {
            try
            {
                string servico = this.Request.Path;
                string xmlConteudo = await base.RecuperarCorpoChamada();

                servicoLog.CriarLog(servico, xmlConteudo, TipoLog.ENTRADA_XML);

                using Schema schema = processardorChamada.CarregarDadosChamadaSoap(servico, xmlConteudo);

                // DEFAULT 404
                if (schema.IsVazio)
                    return await base.ProcessarResposta(NotFound(), servicoLog);

                if (queryParametro == "DEBUG")
                {
                    // DEBUG 
                    var json = conversaoJson.ConverterParaJson(schema);
                    servicoLog.CriarLog(servico, json, TipoLog.DEBUG);
                    return await base.ProcessarResposta(Content(json, "application/json"), servicoLog);
                }

                await processardorChamada.EnviarProcessamento(schema, servicoLog);
                string xmlResposta = conversaoXml.ConverterParaXml(schema);
                // TESTE
                //return Content(xmlResposta, "application/json");

                servicoLog.CriarLog(servico, xmlResposta, TipoLog.RETORNO_XML);
                return await base.ProcessarResposta(Content(xmlResposta, "text/xml"), servicoLog);
            }
            catch (Exception ex)
            {
                return await base.TratamentoErroFatal(ex, servicoLog);
            }
        }
    }
}
