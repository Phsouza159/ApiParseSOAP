using System.Globalization;

namespace WebApiTeste.Registros
{
    public class NumberToWordsResponse
    {

        public NumberToWordsResponse(ulong data)
        {
            this.Data = data;
        }

        private ulong Data { get; set; }


        public string NumberToWordsResult { get => this.Data > 1 ? $"R$ {this.Data} reais" : $"R$ {this.Data} real"; }
    }
}
