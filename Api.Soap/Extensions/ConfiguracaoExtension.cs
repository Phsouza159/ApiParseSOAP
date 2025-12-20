namespace ApiParseSOAP.Extensions
{
    public static class ConfiguracaoExtension
    {
        public static string GetHost(this IConfiguration configuration)
        {
            string par = "URL_HOST";
            return configuration.GetItem(par);
        }

        public static string GetPathServicos(this IConfiguration configuration)
        {
            string par = "PATH_SERVICOS";
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
