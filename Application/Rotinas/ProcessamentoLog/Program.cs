using ApiParseSOAP.Application.IoC;
using Microsoft.Extensions.DependencyInjection;
using ProcessamentoLog.Domain;
using ProcessamentoLog.Domain.Interface;

namespace ProcessamentoLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            ConfigurarDependencias(services);

            if (args.Length < 1)
                throw new ArgumentException("Esperado argumento 1 - Caminho pasta de LOGS");

            if (args.Length < 2)
                throw new ArgumentException("Esperado argumento 2 - Caminho pasta arquivo DATA");

            string pastaLog = args[0];
            string caminhoData = args[1];

            if (!Directory.Exists(pastaLog))
                throw new ArgumentException($"Pasta de LOG não localizada em '{pastaLog}'");

            if (!File.Exists(caminhoData))
                throw new ArgumentException($"Arquivo Data não localizado em '{caminhoData}'");

            var provider = services.BuildServiceProvider();
            using (var processo = provider.GetRequiredService<IProcessoLog>())
            {
                processo.PastaLog    = pastaLog;
                processo.CaminhoData = caminhoData;
                processo.Executar();
            }
        }

        static void ConfigurarDependencias(IServiceCollection services)
        {
            ConfiguracaoDependencias.Configurar(services);

            services.AddScoped<IProcessoLog, ProcessoLog>();
        }
    }
}
