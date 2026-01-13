
import { processadorBase }  from './Processadores/ProcessadorBase.js'
import { fileURLToPath }    from 'url';
import { pathToFileURL }    from 'node:url';
import path                 from 'node:path';


const __filename  = fileURLToPath(import.meta.url);
const __dirname   = path.dirname(__filename);
const filePath    = path.join(__dirname, 'Processadores', 'Conectores');

const args = process.argv;

/**
 * APP START
 */
(async function program() {

    let data = { 
          sucesso  : false
        , mensagem : ''
    }

    try
    {
        if(args.length < 2)
            throw 'Esperado nome do processador.';

        if(args.length < 3)
            throw 'Esperado INPUT data para processamento.';

        var process = new processadorBase()

        process.ProcessadorJs   = pathToFileURL(path.join(filePath, `${args[2]}.js`));
        process.Input           = args[3];

        // EXECUCAO DO PROCESSADOR BASE
        await process.ExecutarProcessador();

        data.sucesso  = process.IsSucesso;
        data.mensagem = process.IsSucesso ? process.Output : process.Mensagem;

    }
    catch (ex)
    {
        data.sucesso  = false;
        data.mensagem = ex.message
    }
    finally {

        // SAIDA DO OUTPUT PARA O CONSOLE
        console.log(JSON.stringify(data));
    }
})()