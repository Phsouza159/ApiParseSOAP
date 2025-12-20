using ApiParseSOAP.Domain;
using Api.Domain.Services;
using ApiParseSOAP.Extensions;
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

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("{servico}")]
    public class ServicoSoapController : BaseController 
    {
        public ServicoSoapController(IConfiguration config)
        {
            Config = config;
        }

        public IConfiguration Config { get; }

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
            )
        {
            string servico = this.Request.Path;
            string xmlConteudo = base.RecuperarCorpoChamada();

            var schema = processardorChamada.CarregarDadosChamadaSoap(servico, xmlConteudo);
            if(!schema.IsVazio)
            {
                if(queryParametro == "DEBUG")
                {
                    // DEBUG 
                    return Content(conversaoJson.ConverterParaJson(schema), "application/json");
                }


                var json = conversaoJson.ConverterParaJson(schema);
                await processardorChamada.EnviarProcessamento(schema);

                // TESTE 
                // return Content(conversao.ConverterParaJson(schema), "application/json");
                //var objeto = conversaoXml.Converter(schema);

                string xmlResposta = conversaoXml.ConverterParaXml(schema);
                return Content(xmlResposta, "text/xml");
            }


            return NotFound();
        }
    }
}
