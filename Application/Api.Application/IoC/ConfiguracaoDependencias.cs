using Api.Application.Facede;
using Api.Application.Integration;
using Api.Application.Integration.Servicos;
using Api.Domain.Conversor;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Facede;
using Api.Domain.Interfaces.Integration;
using Api.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiParseSOAP.Application.IoC
{
    public static class ConfiguracaoDependencias
    {
        public static IServiceCollection ConfigurarIoC(this IServiceCollection services)
        {

            services.AddScoped<IConvercaoJsonParaXml, ConvercaoJsonParaXml>();
            services.AddScoped<IConvercaoXmlParaJson, ConvercaoXmlParaJson>();
            services.AddScoped<IServicoLog, ServicoLog>();
            

            services.AddScoped<IProcessarChamadaSoapFacede, ProcessarChamadaSoapFacede>();
            services.AddScoped<IServicoIntegracaobFacede, ServicoIntegracaoFacede>();
           

            services.AddScoped<IServicoIntegracao, ServicoIntegracao>();
            services.AddScoped<IServicoIntegracao, ServicoIntegracao>();


            services.AddScoped<IServicoPost, ServicoPost>();
            services.AddScoped<IServicoArquivo, ServicoArquivo>();
            services.AddScoped<IServicoProcessadoresNode, ServicoProcessadoresNode>();

            services.AddScoped<IServicoNotImplementation, ServicoNotImplementation>();
            return services;
        }


        public static IServiceCollection ConfigurarPainelLogIoC(this IServiceCollection services)
        {
            services.AddScoped<IRegistroLogFacede, RegistroLogFacede>();

            return services;
        }
    }
}
