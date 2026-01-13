using Api.Domain.Interfaces;

namespace Api.Domain.Configuracao
{
    public class Contrato
    {
        public string Servico { get; set; }

        public string Api { get; set; }

        public string Tipo { get; set; }

        public string TipoAutenticacao { get; set; }

        public IAutenticacao Autenticacao { get; set; }

    }
}
