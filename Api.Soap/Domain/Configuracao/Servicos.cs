using ApiParseSOAP.Domain.Services;
using System.Diagnostics.Contracts;

namespace ApiParseSOAP.Domain.Configuracao
{
    public class Servicos
    {
        public string Nome { get; set; }

        public string UrlHost { get; set; }
        
        public string UrlLocation { get; set; }
        
        public List<Contrato> Contratos { get; set; }
        
        public List<string> ArquivosWsdl { get; set; }

        public Dictionary<string, byte[]> ConteudoArquivos { get; set; }

        internal bool CarregarDados(string raizPasta)
        {
            this.ConteudoArquivos = new Dictionary<string, byte[]>();

            foreach (var arquivo in this.ArquivosWsdl)
            {
                string caminho = Path.Combine(raizPasta, arquivo);

                if (File.Exists(caminho))
                {
                    byte[] data = ServicoArquivosWsdl.RecuperarArquivo(this, caminho);
                    this.ConteudoArquivos.Add(arquivo, data);
                }
                else
                    return false;
            }

            return true;
        }

        internal string GetLocation()
        {
            return this.ConcatenarUrl(this.UrlHost, this.UrlLocation);
        }
        internal string ConcatenarUrl(string baseUrl, string path)
        {
            baseUrl = baseUrl.TrimEnd('/');
            path = path.TrimStart('/');

            // Junta com uma única barra
            return $"{baseUrl}/{path}";
        }

    }
}
