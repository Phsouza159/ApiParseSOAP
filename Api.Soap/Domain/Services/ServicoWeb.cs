using System.Text;

namespace ApiParseSOAP.Domain.Services
{
    public static class ServicoWeb
    {

        internal static async Task Enviar(Configuracao.Servicos servicos, Schema schema)
        {
            var contrato = servicos.Contratos.FirstOrDefault(e => e.Servico.ToLower().Equals(schema.NomeServico.ToLower()));

            if (contrato != null)
            {
                switch (contrato.Tipo)
                {
                    case "POST":
                         await ServicoWeb.Post(contrato, servicos, schema);
                        return;

                    default:
                        break;
                }
            }
        }

        private static async Task Post(Configuracao.Contrato contrato, Configuracao.Servicos servicos, Schema schema)
        {
            using var client = new HttpClient();
            
            var json = schema.ConverterParaJson();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(contrato.Api, content);

            schema.Resultado = await response.Content.ReadAsStringAsync();
            schema.Status = response.StatusCode;
        }
    }
}
