using Api.Domain.Enum;

namespace Api.Domain.Api.Domain
{
    public class RegistroLog
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
    }
}
