using Api.Domain.Enum;

namespace Api.Domain.Api.Domain
{
    internal class RegistroLog
    {
        public RegistroLog()
        {
            this.Conteudo = string.Empty;
        }

        public Guid ID { get; set; }

        public TipoLog TipoLog { get; set; }

        public string Conteudo { get; set; }

        public DateTime Data { get; set; }

        public override string ToString()
        {
            return 
@$"[{this.Data:yyyy-MM-dd HH:mm:ss}] - ID: {this.ID}
TIPO: {this.TipoLog}
----- CONTEUDO - Tamanho {this.Conteudo.Length}:
{this.Conteudo}
---
";
        }
    }
}
