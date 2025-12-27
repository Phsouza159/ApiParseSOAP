using Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Enum;
using Api.Domain.Interfaces;
using System.Text;

namespace Api.Domain.Services
{
    public class ServicoWeb : IServicoWeb
    {
        //  public static IConvercaoJsonParaXml ConvercaoJsonParaXml { get; set; }

        public ServicoWeb(IConvercaoXmlParaJson convercaoXmlParaJson)
        {
            ConvercaoXmlParaJson = convercaoXmlParaJson;
        }

        public IConvercaoXmlParaJson ConvercaoXmlParaJson { get; }

        public async Task Enviar(Schema schema, IServicoLog servicoLog)
        {
            var contrato = schema.Servico.Contratos.FirstOrDefault(e => e.Servico.ToLower().Equals(schema.NomeServico.ToLower()));

            if (contrato != null)
            {
                switch (contrato.Tipo)
                {
                    case "POST":
                         await this.Post(contrato, schema, servicoLog);
                        return;

                    default:
                        break;
                }
            }
        }

        internal async Task Post(Configuracao.Contrato contrato, Schema schema, IServicoLog servicoLog)
        {
            using var client = new HttpClient();
            
            var json = this.ConvercaoXmlParaJson.ConverterParaJson(schema);

            servicoLog.CriarLog(schema.Servico.Nome, json, TipoLog.CHAMADA_JSON);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(contrato.Api, content);
            schema.Resultado = await response.Content.ReadAsStringAsync();

            servicoLog.CriarLog(schema.Servico.Nome, schema.Resultado, TipoLog.RETORNO_JSON);

            schema.Status = response.StatusCode;
        }
    }
}
