
namespace ApiPainel.Domain.DTO
{
    public class ParametrosPesquisaDTO
    {
        public ParametrosPesquisaDTO(DateTime? dataInicio, DateTime? dataFim)
        {
            if (!dataInicio.HasValue)
                throw new ArgumentException("Data Inicio inválido.");

            if (!dataFim.HasValue)
                throw new ArgumentException("Data Fim inválido.");

            this.DataInicio = dataInicio.Value;
            this.DataFim = dataFim.Value;
        }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }
        
    }
}
