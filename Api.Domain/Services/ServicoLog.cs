using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.Extensions;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Api.Domain.Configuracao;

namespace Api.Domain.Services
{
    public class ServicoLog : IServicoLog
    {
        public IConfiguration Configuration { get; }
        public Guid Ticket { get; }

        public ServicoLog(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Ticket = Guid.NewGuid();
        }

        public async Task CriarLog(string servico, string conteudo, TipoLog tipo)
        {
            string name = $"ID-{Ticket}-{Regex.Replace(servico, "[^A-Za-z0-9]", "")}-{tipo}.txt";
            string path = Path.Combine(this.Configuration.GetPathLog(), name);
            await File.WriteAllTextAsync(path, conteudo);
        }
    }
}
