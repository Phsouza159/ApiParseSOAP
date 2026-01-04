using Api.Application;
using Api.Application.Facede;
using Api.Domain.Conversor;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiParseSOAP.Application.IoC
{
    public static class ConfiguracaoDependencias
    {
        public static void Configurar(IServiceCollection services)
        {

            services.AddScoped<IConvercaoJsonParaXml, ConvercaoJsonParaXml>();
            services.AddScoped<IConvercaoXmlParaJson, ConvercaoXmlParaJson>();
            services.AddScoped<IServicoLog, ServicoLog>();
            

            services.AddScoped<IProcessarChamadaSoapFacede, ProcessarChamadaSoapFacede>();
            services.AddScoped<IServicoWebFacede, ServicoWebFacede>();

            services.AddScoped<IServicoIntegracao, ServicoIntegracao>();
            
        }
    }
}
