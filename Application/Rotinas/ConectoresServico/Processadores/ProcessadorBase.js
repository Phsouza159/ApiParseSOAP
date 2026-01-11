
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

        const input = Buffer.from(this.Input, 'base64').toString('utf8')
        let source = JSON.parse(input)

        const modulo = await import(this.ProcessadorJs);
        // EXECUCAO DO WORK
        let data     = await modulo.default(source);

        this.Output = typeof data == 'object' ? JSON.stringify(data) : data;
        this.IsSucesso = true;
    }
}

export { processadorBase }