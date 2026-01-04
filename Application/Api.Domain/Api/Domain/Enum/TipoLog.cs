namespace Api.Domain.Enum
{
    public enum TipoLog
    {
        // ENTRADA COM DADO XML
        ENTRADA_XML = 1,

        // CHAMADA COM DADO JSON
        CHAMADA_JSON,
        // RETORNO COM DADO JSON
        RETORNO_JSON,
        // RESPOSTA COM DADO XML
        RETORNO_XML,

        // RESPOSTA COM DADO XML
        RESPOSTA_XML,

        DEBUG,
        TRACE_ERRO,
        INFO,
    }
}