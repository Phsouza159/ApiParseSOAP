using Api.Domain.Enum;

namespace Api.Domain.Api.Domain
{
    internal class RegistroLog
    {
        public RegistroLog()
        {
            this.Conteudo = string.Empty;
            this.Servico = string.Empty;
        }

        public Guid ID { get; set; }

        public string Servico { get; set; }

        public TipoLog TipoLog { get; set; }

        public string Conteudo { get; set; }

        public DateTime Data { get; set; }

        public override string ToString()
        {
            string data = Ressources.Arquivos.TEMPLETE_DATA_LOG;

            data = string.Join(this.ID.ToString(), data.Split("@ID"));
            data = string.Join(this.Servico, data.Split("@SERVICO"));
            data = string.Join(this.Data.ToString("yyyy-MM-dd HH:mm:ss"), data.Split("@DATA"));
            data = string.Join(this.Conteudo, data.Split("@CONTEUDO"));

            return data;
        }
    }
}
