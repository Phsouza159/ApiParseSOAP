
# API de Parse de SOAP para JSON

API responsável por integrar serviços que consomem SOAP e interagem com outras APIs ou serviços REST/JSON. Realiza a conversão entre XML e JSON, garantindo conformidade com os contratos definidos pelos serviços SOAP.

Também oferece suporte à leitura de arquivos estáticos e à execução de workers em JavaScript, ampliando as possibilidades de processamento e integração dentro do fluxo de comunicação.

### Funcionamento da API

#### Serviços SOAP de XML para JSON

A API oferece suporte ao carregamento de arquivos WSDL e XSD no formato XML, a partir dos quais são obtidas as regras necessárias para validação dos inputs dos serviços em XML. Após a validação com base nos contratos XML, a API realiza o parse de XML para JSON considerando o contrato e as tipagens definidas nos esquemas.
Exemplos
1.  `<xsd:element minOccurs="0" name="Nome" type="xsd:string"/>`
Nesse caso, o `type="xsd:string"` é interpretado como o tipo `string` durante a conversão de XML para JSON.

2.  `<xsd:element minOccurs="0" name="idade" type="xsd:int"/>`
Aqui, o `type="xsd:int"` é interpretado como o tipo `int` na conversão de XML para JSON, gerando erro caso o valor informado não seja um número válido.

#### Fluxo da API

```
+--------------------------------------------------------------------------------------------+

[GET] - Leitura dos contratos XML e WSDL 
[api/servico/{servico}]

+------------+                 +-----------+                         
|            |                 |           |                          
| SERVIÇO A  | --------------> | API PARSE | 
|            |  <--- XML       |       +---|------------------------+        
+------------+                 +-------|---+                        |
                                       |  [Serviço B]               |
                                       |    +--- ServicoB.xml       |  
                                       |    +--- ServicoB.xsd1.xsd  |   
                                       |    +--- ServicoB.xsd2.xsd  |   
                                       +----------------------------+

+-------------------------------------------------------------------------------------------+

[POST] - Execução do serviço
[api/servico/{servico}]

+------------+               +-----------+                          +-----------------------+
|            |               |           |                          |                       |
| SERVIÇO A  | ------------> | API PARSE | ---------- [POST] -----> |  SERVIÇO B (API REST) |
|            |  <-- SAOP --> |           |         <-- JSON -->     |                       |
+------------+               +-+-------+-+                          +-----------------------+
                               |       |
                               |       |
                               |       |                            +-----------------------+
                               |       + ------------ [FILE] -----> |   LER ARQUIVO .JSON   |
                               |                   <-- JSON         +-----------------------|
                               +--------------------- [WORK] -----> | EXECUTAR WORK NODE.JS | 
                                                   <-- JSON -->     +-----------------------|

+-------------------------------------------------------------------------------------------+    

[POST] - Resetar configurações de serviços ou adicionar novos serviços.
[api/recarregarConfiguracao]
    
```


#### Mapeamento dos serviços

No momento de inicialização da API, é realizada a leitura de todas as pastas presentes no diretório configurado na variável `PATH_CONTRATO`. Sempre que uma pasta contém o arquivo `Config.json`, o serviço correspondente — juntamente com seus arquivos XML — é carregado para a memória da API.

- No arquivo de configuração `appsettings.json`, o caminho base para leitura dos contratos é definido pela variável `PATH_CONTRATO`, que controla onde os serviços devem ser localizados.
- A organização dos serviços e suas configurações segue a estrutura de pastas apresentada a seguir:

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


| Tipo | Descrição | Documentação|
| :---: | :--- | :---: |
| [POST] | Requisições WEB do tipo REST/POST | - |
| [FILE] | Leitura de arquivos  | - |
| [WORK] | Execução de work em `Node.js` em arquivos pré-configurados | [Modelo de configuração NODE.JS](Documentacao/configuracao-work.md)  |



