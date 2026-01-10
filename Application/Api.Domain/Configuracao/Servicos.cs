using Api.Domain.Enum;
using Api.Domain.Helper;
using Api.Domain.Services;

namespace Api.Domain.Configuracao
{
    public class Servicos
    {
        public Servicos()
        {
        }

        public string Nome { get; set; }

        public string UrlHost { get; set; }
        
        public string UrlLocation { get; set; }
        
        public string Prefixo { get; set; }

        public bool IgnorarNulo { get; set; }

        public string PrefixoImportacaoRegex { get; set; }

        public List<Contrato> Contratos { get; set; }
        
        public List<string> ArquivosWsdl { get; set; }

        public Dictionary<string, byte[]> ConteudoArquivos { get; set; }

        public bool IsImportacao { get => this.ConteudoArquivos.Any(e => e.Key.Contains(".xsd")); }

        public string PastaRaizServico { get; set; }

        internal bool CarregarDados(string raizPasta)
        {
            this.PastaRaizServico = raizPasta;
            this.ConteudoArquivos = new Dictionary<string, byte[]>();
            bool isArquivosWsdl = this.CarregarArquivosWsdl(raizPasta);
            return isArquivosWsdl;
        }

        #region CARREGAR ARQUIVOS WSDL

        internal bool CarregarArquivosWsdl(string raizPasta)
        {
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

        #endregion

        internal string GetLocation()
        {
            return StringHelper.ConcatenarUrl(this.UrlHost, this.UrlLocation);
        }
    }
}
