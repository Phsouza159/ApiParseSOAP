
/**
 * 
 * @param {object} input 
 * @param {Array<string>} log 
 * @returns 
 */
async function webBenchmark(input, log)
{
    let data = input.DadosEntrada.numbers
       , sum = data.reduce((accumulator, currentValue) => accumulator + currentValue, 0);

    return { 
        SumResponse : {
            SumResult : sum
        }
    }
}

export default webBenchmark