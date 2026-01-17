
import { processadorBase }  from './Processadores/ProcessadorBase.js'
import { pathToFileURL }    from 'node:url';
import path                 from 'node:path';

// ARUGMENTOS DA EXECUCAO
const args = process.argv;

/**
 * APP START
 */
(async function program() {

    /**
     * ENVELOPE DE RESPOSTA DO app.js
     */
    let data = { 
          sucesso  : false
        , mensagem : ''
        , log      : []
    }

    try
    {
        if(args.length < 2)
            throw 'Esperado nome do processador.';

        if(args.length < 3)
            throw 'Esperado INPUT data para processamento.';

        var process             = new processadorBase()

        const caminhoRaiz       = args[1].replace('app.js', '')                             // PEGAR CAMINHO DO ARQUIVO APP.JS 
        const filePath          = path.join(caminhoRaiz, 'Processadores', 'Conectores');    // MONTAR PATH PARA PASTA CONECTORES

        process.ProcessadorJs   = pathToFileURL(path.join(filePath, `${args[2]}.js`));      // NONTAR NOME ARQUIVO WOKR
        process.Input           = args[3];

        await process.ExecutarProcessador();                                                // EXECUTAR PROCESSADOR

        data.sucesso            = process.IsSucesso;
        data.mensagem           = process.IsSucesso ? process.Output : process.Mensagem;    // RECUPERAR MENSAGEM PROCESSAMENTO
        data.log.push(...process.Log)

    }
    catch (ex)
    {
        data.sucesso  = false;
        data.mensagem = ex.message
    }
    finally {

        // SAIDA COM OUTPUT PARA O CONSOLE
        console.log(JSON.stringify(data));
    }
})()