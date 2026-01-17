
using Api.Domain.Services;
using Api.Domain.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.UserSecrets;
using Api.Domain.Interfaces;
using ApiParseSOAP.Application.IoC;
using Api.Domain.Helper;

namespace ApiParseSOAP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.ConfigurarIoC();

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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MinhaPoliticaCORS", policy =>
                {
                    policy
                        .AllowAnyOrigin()   // Permite requisições de qualquer domínio
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();


            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();

            var log = provider.GetRequiredService<IServicoLog>();

            ServicoArquivosWsdl.PastaWsdl = Path.Combine(config.GetPastaContratos(), "Contratos");
            ServicoArquivosWsdl.PathHost  = config.RecuperarHostServico();

            ServicoArquivosWsdl.CarregarArquivosConfiguracao(log);
            ProcessadoresHelper.CarregarProcessadores();

            log.Save();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("MinhaPoliticaCORS");
               // app.UseSwagger();
               // app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
