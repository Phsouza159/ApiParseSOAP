async function incluirPreAvisoSinistro(input) {
  
    let data = {
        incluirPreAvisoSinistroResponse: {
            retorno: {
                codRetorno: '37',
                desMensagemSistema: "Erro ao tentar gravar os dados pela stored procedure.",
                desMensagemAmigavel: "SXSIP904 CONTRATO NAO ENCONTRADO NA EMISSAO Divide by zero error encountered."
            }
        }
    }

    return data
}

export default incluirPreAvisoSinistro