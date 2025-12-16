using Api.Domain.Helper;
using Api.Domain.Services;

namespace Api.Domain.Configuracao
{
    public class Servicos
    {
        public string Nome { get; set; }

        public string UrlHost { get; set; }
        
        public string UrlLocation { get; set; }
        
        public string Prefixo { get; set; }

        public List<Contrato> Contratos { get; set; }
        
        public List<string> ArquivosWsdl { get; set; }

        public Dictionary<string, byte[]> ConteudoArquivos { get; set; }

        public bool IsImportacao { get => this.ConteudoArquivos.Any(e => e.Key.Contains(".xsd")); }

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
            return StringHelper.ConcatenarUrl(this.UrlHost, this.UrlLocation);
        }
    }
}
