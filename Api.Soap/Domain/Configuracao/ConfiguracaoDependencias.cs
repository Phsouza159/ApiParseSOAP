using Api.Domain.Conversor;
using Api.Domain.Facede;
using Api.Domain.Interfaces;
using Api.Domain.Services;

namespace ApiParseSOAP.Domain.Configuracao
{
    public static class ConfiguracaoDependencias
    {
        public static void Configurar(IServiceCollection services)
        {

            services.AddScoped<IConvercaoJsonParaXml, ConvercaoJsonParaXml>();
            services.AddScoped<IConvercaoXmlParaJson, ConvercaoXmlParaJson>();

            services.AddScoped<IProcessarChamadaSoapFacede, ProcessarChamadaSoapFacede>();

            services.AddScoped<IServicoWeb, ServicoWeb>();
            
        }
    }
}
