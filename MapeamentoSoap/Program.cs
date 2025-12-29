


using System;
using System.Xml;
using System.Xml.Schema;

//string pathCaminhoServico = args.Length < 1 ? throw new ArgumentException("Argumento 1 - Caminho pasta obrigatorio") : args[0];
//string urlServico         = args.Length < 2 ? throw new ArgumentException("Argumento 2 - Caminho Servico WSDL") : args[1];
//string urlServico         = args.Length < 2 ? throw new ArgumentException("Argumento 2 - Caminho Servico WSDL") : args[1];
//string pathCaminhoServico = "http://localhost:5500/SXWSV0030_ContratoSistema/SXWSV0030_ContratoSistema.xml";

using var client = new HttpClient();

Dictionary<string, string> arquivos = new Dictionary<string, string>();


async Task<bool> GetWsdl(string urlServico)
{
    var resultado = await client.GetAsync(urlServico);

    Console.WriteLine($"Recuperando XML: {urlServico}");

    if (resultado.StatusCode == System.Net.HttpStatusCode.OK
        && resultado.RequestMessage != null
        && resultado.RequestMessage.RequestUri != null)
    {
        string[] segments = resultado.RequestMessage.RequestUri.Segments;
        string nomeArquivo = segments[segments.Length - 1];
        string conteudo    = await resultado.Content.ReadAsStringAsync();

        ValidarDocumentoXML(conteudo);

        arquivos.Add(nomeArquivo, conteudo);
        return true;
    }

    return false;
}

async Task GetXsd(string urlServico)
{
    Uri uri = new Uri(urlServico);
    string[] segments = uri.Segments;
    string ultimoSegmento = segments[segments.Length - 1];
    string copiaUltimoSegmento = ultimoSegmento;
    int contador = 1;

    if (ultimoSegmento.Length > 4)
    {
        string anexo = ultimoSegmento.Substring(ultimoSegmento.Length - 4, 4);
        if(anexo.ToLower().Equals(".xml"))
        {
            // REMOVER ANEXO - XML
            ultimoSegmento = ultimoSegmento.Substring(0, ultimoSegmento.Length - 4);
        }
    }

    do
    {
        string url = $"{urlServico.Replace(copiaUltimoSegmento, ultimoSegmento)}.xsd{contador}.xsd";
       
        // CARREGAR ARQUIVO XSD
        bool sucesso = await GetWsdl(url);

        // FALHA = BREAK
        if (!sucesso) break;

        // PROXIMO ARQUIVO XSD
        contador += 1;

    } while (true);
}

string RecuperarVariavel(string texto)
{
    Console.WriteLine(texto);
    return Console.ReadLine();
}

async Task CriarArquivos()
{
    string nomeServico = RecuperarVariavel("Informe o nome do Serviço:");
    string pastaRaiz = string.Empty;
    string path = string.Empty;

    arquivos.Add("config.json", MapeamentoSoap.Arquivos.Resource.MODELO_CONFIG
                .Replace("{NOME}", nomeServico)
                .Replace("{ARQUIVOS}", string.Join(",", arquivos.Select(e => $"\t\"{e.Key}\"{Environment.NewLine}"))));
    
    do
    {
        pastaRaiz = RecuperarVariavel("Informe a pasta raiz do Serviço:");

        if (Directory.Exists(pastaRaiz))
            break;
        
        Console.WriteLine($"Caminho não existe: {pastaRaiz}");

    } while (true);

    path = Path.Combine(pastaRaiz, nomeServico);

    if (Directory.Exists(path))
        throw new ArgumentException($"Serviço já registrado: {nomeServico}");

    Directory.CreateDirectory(path);

    foreach (var arquivo in arquivos)
    {
        string pathArquivo = Path.Combine(path, arquivo.Key);
        Console.WriteLine($"Criando arquivo: {pathArquivo}");
        await File.WriteAllTextAsync(pathArquivo, arquivo.Value);
    }
}

void ValidarDocumentoXML(string conteudo)
{
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.LoadXml(conteudo);
    // INVALIDO - GERAR EXCEPTION
}

string pathCaminhoServico = RecuperarVariavel("Informe a URL do do serviço WSDL:");

// START
await GetWsdl(pathCaminhoServico);
await GetXsd(pathCaminhoServico);
await CriarArquivos();