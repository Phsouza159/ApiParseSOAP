using Api.Domain.Configuracao;
using Api.Domain.Helper;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using ApiParseSOAP.Application.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Api.Validacoes.Base
{
    public class BaseTeste
    {
        private IServiceCollection Services { get; set; }

        protected ServiceProvider Provider { get; set; }

        public BaseTeste()
        {
        }

        [TestInitialize]
        public void CarregarDenpendencias()
        {
            this.Services = new ServiceCollection();
            this.Services.ConfigurarIoC();
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            this.Services.AddSingleton<IConfiguration>(configuration);

            this.Provider = this.Services.BuildServiceProvider();
        }

        protected void CarregarDadosContratosServicos()
        {
            var log = this.Provider.GetRequiredService<IServicoLog>();

            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);

            ServicoArquivosWsdl.PastaWsdl = Path.Combine(fileInfo.Directory.FullName, "Contratos");
            ServicoArquivosWsdl.PathHost = "SERVICO_TESTE";

            ServicoArquivosWsdl.CarregarArquivosConfiguracao(log);
            ProcessadoresHelper.CarregarProcessadores();
        }
    }
}
