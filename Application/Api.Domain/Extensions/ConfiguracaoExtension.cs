using Microsoft.Extensions.Configuration;

namespace Api.Domain.Extensions
{
    public static class ConfiguracaoExtension
    {
        public static string RecuperarHostServico(this IConfiguration configuration)
        {
            string par = "URL_HOST";
            return string.Format("{0}/api/Servico", configuration.GetItem(par));
        }

        public static string GetPathServicos(this IConfiguration configuration)
        {
            string par = "PATH_SERVICOS";
            return configuration.GetItem(par);
        }

        public static string GetPathLog(this IConfiguration configuration)
        {
            string par = "PATH_LOG";
            return configuration.GetItem(par);
        }

        private static string GetItem(this IConfiguration configuration, string key)
        {
            string? value = configuration[key]?.ToString();

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"Valor {key} inválido: {value}");

            return value;
        }
    }
}
