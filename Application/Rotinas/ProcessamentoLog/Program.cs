using Api.Data.Repository;
using Api.Domain.Interfaces.Repository;
using ApiParseSOAP.Application.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessamentoLog.Domain;
using ProcessamentoLog.Domain.Interface;
using Api.Domain.Extensions;

namespace ProcessamentoLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            ConfigurarDependencias(services);

            var provider = services.BuildServiceProvider();

            IConfiguration config = provider.GetService<IConfiguration>() 
                    ?? throw new ArgumentException("Falha ao carregar 'IConfiguration'.");

            string pastaLog    = config.GetPathLog();
            string caminhoData = config.GetPastaDatabase();

            if (!Directory.Exists(pastaLog)) throw new ArgumentException($"Pasta de LOG não localizada em '{pastaLog}'");
            if (!File.Exists(caminhoData))   throw new ArgumentException($"Arquivo Data não localizado em '{caminhoData}'");

            using var processo   = provider.GetRequiredService<IProcessoLog>();

            processo.PastaLog    = pastaLog;
            processo.CaminhoData = caminhoData;
            processo.Executar();
        }

        static void ConfigurarDependencias(IServiceCollection services)
        {
            services.ConfigurarIoC();
            services.AddScoped<IProcessoLog, ProcessoLog>();
            services.AddScoped<IRepositorioData, RepositorioData>();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}
