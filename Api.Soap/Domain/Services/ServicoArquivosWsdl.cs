

using ApiParseSOAP.Domain.Configuracao;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;

namespace ApiParseSOAP.Domain.Services
{
    public static class ServicoArquivosWsdl
    {
        static string VAR_EXTENSION_WSDL = "wsdl";

        internal static string PastaWsdl { get; set; }

        internal static string PathHost { get; set; }

        internal static List<Servicos> Configuracacoes { get; set; } = [];

        #region TRATAR ARQUIVOS WSDL

        internal static byte[] RecuperarArquivo(Servicos servicos, byte[] data)
        {
            string lines = Encoding.UTF8.GetString(data);
            return RecuperarArquivo(servicos, lines.Split("\r\n"));
        }

        internal static byte[] RecuperarArquivo(Servicos servicos, string pathServico)
        {
            string[] lines = File.ReadAllLines(pathServico);
            return RecuperarArquivo(servicos, lines);
        }

        internal static byte[] RecuperarArquivo(Servicos servicos, string[] lines)
        {
            string tns = servicos.UrlHost; //servicos.ConcatenarUrl(servicos.UrlHost, servicos.Nome);
            string tnsValue = PathHost; // servicos.ConcatenarUrl(PathHost, servicos.Nome);

            string targetnamespace = servicos.UrlHost;  //servicos.ConcatenarUrl(servicos.UrlHost, servicos.Nome);
            string targetnamespaceValue = PathHost; // servicos.ConcatenarUrl(PathHost, servicos.Nome);

            string location = servicos.GetLocation();
            string locationValue = servicos.ConcatenarUrl(PathHost, servicos.UrlLocation); 

            for (int i = 0; i < lines.Length; i += 1)
            {
                string line = lines[i];

                if (line.Contains($"xmlns:tns=\"{tns}\""))
                    line = line.Replace($"xmlns:tns=\"{tns}\"", $"xmlns:tns=\"{tnsValue}\"");

                if (line.Contains($"targetNamespace=\"{targetnamespace}\""))
                    line = line.Replace($"targetNamespace=\"{targetnamespace}\"", $"targetNamespace=\"{targetnamespaceValue}\"");

                if (line.Contains("location="))
                {
                    string locationItem = location;

                    line = TratarEndereloLocal(line, location, locationValue);
                    line = TratarEndereloLocal(line, locationItem.Replace("http", "https"), locationValue);
                }

                lines[i] = line;
            }

            return Encoding.UTF8.GetBytes(string.Join("\r\n", lines));
        }

        internal static string TratarEndereloLocal(string line, string localtion, string locationValue)
        {
            if (line.Contains($"location=\"{localtion}\""))
                line = line.Replace($"location=\"{localtion}\"", $"location=\"{locationValue}\"");

            return line;
        }

        #endregion


        internal static string ResoverNomeArquivo(string servico)
        {
            servico = servico.Replace("/", "");

            if(servico.Contains("."))
            {
                string[] s = servico.Split('.');
                return string.Concat(string.Join(".", s.Where(x => x != s[s.Length -1])), ".xml");
            }

            return $"{servico}.xml";
        }


        internal static XmlDocument TransformarXml(string xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            return doc;
        }

        internal static Schema CarregarXml(XmlDocument contrato, XmlDocument documento)
        {
            var schema = new Schema();
            schema.Documento = documento;
            schema.Contrato = contrato;
            schema.Carregar();

            return schema;
        }

        #region CARREGAR CONFIGURACAO

        internal static void CarregarArquivosConfiguracao()
        {
            string[] diretorios = Directory.GetDirectories(PastaWsdl);

            foreach (var diretorio in diretorios)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(diretorio);
                string caminhoArquivoConfig = Path.Combine(diretorio, "config.json");
                if (directoryInfo.Exists && File.Exists(caminhoArquivoConfig))
                {
                    Servicos? servico = JsonConvert.DeserializeObject<Servicos>(File.ReadAllText(caminhoArquivoConfig));

                    if(servico != null 
                        && !Configuracacoes.Any(e => e.Nome == directoryInfo.Name)
                        && servico.CarregarDados(diretorio))
                    {
                        servico.Nome = directoryInfo.Name;
                        Configuracacoes.Add(servico);
                    }
                }
            }
        }

        internal static Servicos? RecuperarServico(string servico)
        {
            servico = servico.Replace("/", "");
            return Configuracacoes.FirstOrDefault(e => e.UrlLocation.ToLower().Equals(servico.ToLower()));
        }

        #endregion
    }
}
