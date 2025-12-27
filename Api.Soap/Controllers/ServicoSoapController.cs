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

                if(servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
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
                string xmlConteudo = base.RecuperarCorpoChamada();

                var schema = processardorChamada.CarregarDadosChamadaSoap(servico, xmlConteudo);
                if (!schema.IsVazio)
                {
                   await servicoLog.CriarLog(servico, xmlConteudo, TipoLog.ENTRADA_XML);

                    if (queryParametro == "DEBUG")
                    {
                        // DEBUG 
                        var json = conversaoJson.ConverterParaJson(schema, isDadosEntrada: true);
                        await servicoLog.CriarLog(servico, json, TipoLog.DEBUG);
                        return Content(json, "application/json");
                    }

                    await processardorChamada.EnviarProcessamento(schema, servicoLog);
                    string xmlResposta = conversaoXml.ConverterParaXml(schema);
                    // TESTE
                    //return Content(xmlResposta, "application/json");

                    await servicoLog.CriarLog(servico, xmlResposta, TipoLog.RETORNO_XML);

                    return Content(xmlResposta, "text/xml");
                }
            }
            catch (Exception ex)
            {
                return await base.TratamentoErroFatal(ex);
            }

            // DEFAULT 404
            return NotFound();
        }
    }
}
