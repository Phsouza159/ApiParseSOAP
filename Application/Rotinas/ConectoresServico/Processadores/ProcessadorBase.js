
import fs from 'node:fs';

class processadorBase {

    Input     = ''
    Output    = ''
    
    Mensagem  = ''
    IsSucesso = false

    ProcessadorJs = ''
    Log       = []

    async ExecutarProcessador() {

        if (!fs.existsSync(this.ProcessadorJs)) {
            this.Mensagem = `Arquivo de processamento não localizado em: ${this.ProcessadorJs}`;
            return;
        }

        const input = Buffer.from(this.Input, 'base64').toString('utf8')
        let source = JSON.parse(input)

        const modulo = await import(this.ProcessadorJs);

        this.Log.push('Iniciando processamento WORK: webBenchmark')

        // EXECUCAO DO WORK
        let data     = await modulo.default(source, this.Log);

        this.Log.push('Executado com sucesso WORK: webBenchmark')

        this.Output = typeof data == 'object' ? JSON.stringify(data) : data;
        this.IsSucesso = true;
    }
}

export { processadorBase }