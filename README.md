
# API de Parse de SOAP para JSON

Api responsável por fazer integração de serviços de API que consume serviços SOAP e interagem com outras API ou Servicos com REST/JSON. Fazendo a comunicão de XML para JSON respeitando os contratos definidos nos serviços SOAP.

### Funcionamento da API

#### Servicos SOAP de XML para JSON

Api tem suporte para carregamento de arquivos `WSDL` e `XSD` no formato de `XML` onde são carregados as regras de suporte para validação dos INPUT dos serivços em XML. Feito o tratamento do INPUT atráves dos contratos XML, API faz o parse de XML para o JSON com base no contrato e nas tipagens do objeto.

Exemplos

1.  `<xsd:element minOccurs="0" name="Nome" type="xsd:string"/>` onde o `type="xsd:string"` é tratado como tipagem de tipo `string` na coversão de XML para JSON

2. `<xsd:element minOccurs="0" name="idade" type="xsd:int"/>` onde o `type="xsd:int"` é tratado como tipagem de tipo `int` na coversão de XML para JSON. Apresentando um erro de conversão caso o valor não seja um valor númerico válido.

#### Mapeamento dos serviços

No momento de start da API, e feito a leitura de todas as pastas que ficam dentro do diretorio configurado na `PATH_CONTRATO`. Se dentro das pasta é localizado o arquivo `Config.json`, o serivço e os seus arquivos XML são carregados para a memória da API.

- No arquivo de configuração do projeto `appsettings.json` existe o controle do PATH da pasta atráves da variável `PATH_CONTRATO`

- Organização dos serviços e as configurações são feitos atráves do esquema de pasta a seguir:

```
PASTA DE CONTRATOS:               // PASTA CONFIGURADA PARA OS CONTRATOS [PATH_CONTRATO]
    |                             
    +--[SERVIÇO A]                // PASTA COM O SERVIÇO
    |    |
    |    +--- Config.json          // ARQUIVO DE CONFIGURAÇÃO DO SERVIÇO
    |    +--- Servico A.xml        // CONTRATO WSDL
    |    +--- Servico A.xsd1.xsd   // CONTRATO DE IMPORTAÇÃO XSD
    |    +--- Servico A.xsd2.xsd   // CONTRATO DE IMPORTAÇÃO XSD
    |    ....
    +--[SERVIÇO B]
    ....
```

* Arquivo de configuração

O arquivo `Config.json` é o arquivo de entrada para a configuração e controle dos serviços mapeados - [Modelo de arquivo JSON](Documentacao/configuracao-servico.md)


#### Integração dos serviços
As integrações da API tem suporte a três modelos e são configuradas nos arquivos de configuração `Config.json` no item `"Tipo": "[TIPO]"`.

Tipos suportados:

* POST - Requisições WEB do tipo REST/POST
  * Para autentição de API, existe o parametro de redirecionamento de tokens `{ "Autenticacao": "REDIRECIONAR_AUTENTICACAO" }`   
  * Autenticação do Serviço A e carregada para o Serviço B

* FILE - Leitura de arquivos `.json`

* WORK - Execução de work processamento em `Node.js` em arquivos pré-configurados - [Modelo de configuração NODE.JS](Documentacao/configuracao-servico.md) 



```
+------------+               +-----------+                          +-----------------------+
|            |               |           |                          |                       |
| SERVICO A  | ------------> | API PARSE | ---------- [POST] -----> |  SERVIÇO B (API REST) |
|            |  <-- SAOP --> |           |         <-- JSON -->     |                       |
|+-----------+               +-|-------|-+                          +-----------------------+
                               |       |
                               |       |
                               |       |                            +-----------------------+
                               |       + ------------ [FILE] -----> |   LER ARQUIVO .JSON   |
                               |                   <-- JSON         +-----------------------|
                               +--------------------- [WORK] -----> | EXECUTAR WORK NODE.JS | 
                                                   <-- JSON -->     +-----------------------|
                                      
```

