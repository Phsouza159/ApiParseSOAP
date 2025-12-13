using ApiParseSOAP.Domain;
using ApiParseSOAP.Domain.Services;
using ApiParseSOAP.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;
using System.Xml;

namespace ApiParseSOAP.Controllers
{
    [ApiController]
    [Route("{servico}")]
    public class ServicoSoapController : Controller
    {
        public ServicoSoapController(IConfiguration config)
        {
            Config = config;
        }

        public IConfiguration Config { get; }

        [HttpGet]
        public IActionResult Get(string servico, [FromQuery] string? wsdl)
        {
            Domain.Configuracao.Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

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
        public async Task<IActionResult> Post()
        {
            string servico = this.Request.Path;
            Domain.Configuracao.Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if(servicoConfiguracao != null)
            {
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servicoConfiguracao.Nome);

                if (servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
                {
                    using var reader = new StreamReader(Request.Body);
                    string xmlConteudo = reader.ReadToEnd();

                    byte[] data = servicoConfiguracao.ConteudoArquivos[nomeTratado];
                    string xmlContrato = Encoding.UTF8.GetString(data);

                    var schema = ServicoArquivosWsdl.CarregarXml(
                              ServicoArquivosWsdl.TransformarXml(xmlContrato)
                            , ServicoArquivosWsdl.TransformarXml(xmlConteudo));

                    await ServicoWeb.Enviar(servicoConfiguracao, schema);
                    return Content(schema.Resultado, "application/json");
                }
            }

            return NotFound();
        }
    }
}
