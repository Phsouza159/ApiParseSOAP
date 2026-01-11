
# Configuração de Works em JS


```
[PROJETO - ConectoresServico]:          
    |                             
    +--- app.js             
    |    
    +--- [Processadores]
        |
        +---[Conectores]
        |    |
        |    +--------------+
        |    | PROCESSADORES .js CONFIGURADOS  
        |    +--------------+
        |
        +---- ProcessadorBase.js
    ....
```

Arquivo principal de entrada `app.js`

### Funcionamento da execução do Work

#### API C# execução processo `node.js` em background 

```
        internal async Task Processador(Contrato contrato, Schema schema, EnvelopeEnvio envelope, IServicoLog servicoLog)
        {
            string appNode = Configuration.GetAppNode();

            servicoLog.CriarLog(schema.Servico.Nome, $"Iniciando processador: {contrato.Api}", TipoLog.INFO);
            string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(envelope.ConteudoEnvio));

            var processo = new Process();
            processo.StartInfo.FileName = "node";
            processo.StartInfo.Arguments = $"{appNode} {contrato.Api} '{data}'";
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;

            processo.Start();

            string dataProcessamento = await processo.StandardOutput.ReadToEndAsync();
            string dataErro = await processo.StandardError.ReadToEndAsync();

            await processo.WaitForExitAsync();

            this.TratarRetornoProcessador(
                  contrato
                , schema
                , envelope
                , servicoLog
                , dataProcessamento
                , dataErro
            );
        }
```

Execução de node via `Process`
> `processo.StartInfo.Arguments = $"{appNode}  {contrato.Api} '{data}'";` 
>> `appNode` - Valor configurado em `PATH_APP_NODE` apontando caminho do `app.js` \
>> `contrato.Api` - Nome dor Work sem a extensão `.js` a ser executado \
>> `data` - INPUT do objeto XML de entrada que está convertido em JSON no formato de base64


#### Node.js executando `app.js`

Node executa o arquivo `app.js` que por sua vez executa a instância do classe `ProcessadorBase.js`
```
 [...]
 var process = new processadorBase()

 process.ProcessadorJs   = pathToFileURL(path.join(filePath, `${args[2]}.js`));
 process.Input           = args[3];

 await process.ExecutarProcessador();

 [...]

 console.log(JSON.stringify(data));
 [...]
```
Execução do `app.js`
> `args[2]` - Nome do work \
> `args[3]` - INPUT em base64\

> `process.ProcessadorJs   = pathToFileURL(path.join(filePath, '${args[2]}.js'));` 
>> `pathToFileURL(path.join(filePath, '${args[2]}.js'))` - Montagem completa do caminho do Work \
>> `await process.ExecutarProcessador();` - Execução do Processador base encapsulado \
>> `data` - INPUT do objeto XML de entrada que está convertido em JSON no formato de base64 \

> `console.log(JSON.stringify(data))` Saída output para o console com o resultado do processamento 

Classe de Processador Base

```
class processadorBase {

    [...]

    async ExecutarProcessador() {

        [...]

        const input = Buffer.from(this.Input, 'base64').toString('utf8')
        let source = JSON.parse(input)

        const modulo = await import(this.ProcessadorJs);
        let data     = await modulo.default(source);

        [...]
    }
}
```
Execução do `processadorBase.js`
> `const input = Buffer.from(this.Input, 'base64').toString('utf8')` - Conversão de base64 para texto puro \
> `let source = JSON.parse(input)` - parse do INPUT de `string` para `object` 

> `const modulo = await import(this.ProcessadorJs);` - Importação do work \
> `let data     = await modulo.default(source);` - Execução do Work com os dados do INPUT


#### Exemplo de modelo de work
- Work para somar dois valores 
  - Nome do Work `somarDoisValoresWork`
  - Arquivo `Processadores\Conectores\somarDoisValoresWork.js`
  - Input `{ valorA: 1, valorB: 2 }`
  - Output `{ valor: 3 }`

```
async function somarDoisValoresWork(input) {

    let resultado = input.valorA + input.valorB

    return {
        valor : resultado
    }
}

export default somarDoisValoresWork
```
>