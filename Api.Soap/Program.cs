
using ApiParseSOAP.Domain.Configuracao;
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ApiParseSOAP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            ConfiguracaoDependencias.Configurar(builder.Services);

            builder.Services.AddControllers()
                        .AddXmlSerializerFormatters();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
           // builder.Services.AddSwaggerGen();

            // If using Kestrel:
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            var app = builder.Build();


            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();

            ServicoArquivosWsdl.PastaWsdl = Path.Combine(config.GetPathServicos(), "Contratos");
            ServicoArquivosWsdl.PathHost = config.GetHost();

            ServicoArquivosWsdl.CarregarArquivosConfiguracao();


            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
