
import path from 'node:path';
import fs from 'node:fs';
import { fileURLToPath } from 'url';
import { pathToFileURL } from 'node:url';

class processadorBase {

    Input     = ''
    Output    = ''
    
    Mensagem  = ''
    IsSucesso = false

    ProcessadorJs = ''

    async ExecutarProcessador() {

        if (!fs.existsSync(this.ProcessadorJs)) {
            this.Mensagem = 'Arquivo de processamento não localizado.';
            return;
        }

        const modulo   = await import(this.ProcessadorJs);
        this.Output    = await modulo.default(this.Input);
        this.IsSucesso = true;
    }
}

export { processadorBase }