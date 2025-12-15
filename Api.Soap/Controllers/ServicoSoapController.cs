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
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servicoConfiguracao.Nome);

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

                [FromServices] IProcessarChamadaSoapFacede processardorChamada
                , [FromServices] IConvercaoJsonParaXml convercaoJsonParaXml
            )
        {
            string servico = this.Request.Path;
            string xmlConteudo = base.RecuperarCorpoChamada();

            var schema = processardorChamada.CarregarDadosChamadaSoap(servico, xmlConteudo);
            if(!schema.IsVazio)
            {
                await processardorChamada.EnviarProcessamento(schema);

                var objeto = convercaoJsonParaXml.Converter(schema);

                return Content(schema.Resultado, "application/json");
            }


            return NotFound();
        }
    }
}
