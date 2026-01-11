
# Arquivo de configuração `Config.json`

`UrlHost` - URL original do arquivo WSDL
> Exemplo de localização: `<soap11:address location="https://site/ServicoA" />`

`UrlLocation` -  Nome do serviço configurado - Mesmo nome da pasta onde está os demais arquivos 

`Prefixo` - Valor estatico e fixo com `XSD`

`IgnorarNulo` - Valor boleano `True` ou `False` para sinalização se os arquivos JSON serão tratados carregandos as propriedades `NULL`
> exemplo `True` para `Nome : Null` arquivo JSON com `{ }` \
> exemplo `False` para `Nome : Null` arquivo JSON com `{ "Nome" : Null }`

`PrefixoImportacaoRegex` - Prefixos de arquivos de importação do itens `XSD` com tratamento REGEX para resolução de importação de arquivos de contrato.
> `xmlns:bons1="http://site/exemplo/bo"` - valor importação 'bons1' - bons\\\d+ \
> `xmlns:bons2="http://site/exemplo/bo"` - valor importação 'bons2' - bons\\\d+ \
> `xmlns:tns="http://site/exemplo/bo"`   - valor importação 'tns' - tns  

`Contratos` - Array de objetos de configuração para os metados exposto do serviço:
> `Servico` Nome do Serviço/Método \
> `Api` URL ou PATH do apontamento do Serviço/Método \
> `Tipo` tipo do Processamento do Serviço/Método `['POST','PROCESSADOR_NODE','FILE']` \
> `Autenticacao` Para o tipo `POST` onde existe autenticação `['REDIRECIONAR_AUTENTICACAO']` 

`ArquivosWsdl` - Array de `string` contendo os nomes do arquivos XML do `WSDL` e `XSD`
> Arquivo `WSDL` sendo o de index 0 \
> Arquivos `XSD` sendo o de index > 0

## Modelo de arquivo e estrutura da pasta
```
{
    "UrlHost": "https://site/ServicoA",                   
    "UrlLocation": "ServicoA",
    "Prefixo": "xsd",
    "IgnorarNulo" : false,
    "PrefixoImportacaoRegex": "^(bons\\d+|tns|tns\\d+|Q\\d+)$",
    "Contratos": [
        {
            "Servico": "ServicoTeste1",
            "Api": "https://siteWeb/api/teste",
            "Tipo": "POST",
            "Autenticacao": "REDIRECIONAR_AUTENTICACAO"
        },
        {
            "Servico": "ServicoTeste2",
            "Api": "WorkTest",
            "Tipo": "PROCESSADOR_NODE",
        },
		{
            "Servico": "ServicoTeste3",
            "Api": "C:\\..\\ServicoA\\arquivo.json",
            "Tipo": "FILE",
        }
    ],
    "ArquivosWsdl": [
       "ServicoA.xml"
     , "ServicoA.xsd1.xsd"
     , "ServicoA.xsd2.xsd"
    ]
}
 
```
```
PASTA DE CONTRATOS:               // PASTA CONFIGURADA PARA OS CONTRATOS [PATH_CONTRATO]
    |                             
    +--[SERVICO A]                // PASTA COM O SERVICO
    |    |
    |    +--- Config.json         // ARQUIVO DE CONFIGURAÇÃO DO SERVICO
    |    +--- ServicoA.xml        // CONTRATO WSDL
    |    +--- ServicoA.xsd1.xsd   // CONTRATO DE IMPORTAÇÃO XSD
    |    +--- ServicoA.xsd2.xsd   // CONTRATO DE IMPORTAÇÃO XSD
    |    ....
    +--[SERVICO B]
    ....
```

