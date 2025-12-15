namespace WebApiTeste.Registros
{
    public class NumberToDollarsResponse
    {
        public NumberToDollarsResponse(ulong data)
        {
            this.Data = data;
        }

        private ulong Data { get; set; }


        public string NumberToDollarsResult { get => this.Data > 1 ? $"R$ {this.Data} reais" : $"R$ {this.Data} real"; }
    }
}
