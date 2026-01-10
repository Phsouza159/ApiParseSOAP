using System.ComponentModel;

namespace Api.Domain.Enum
{
    public enum TiposProcessadores
    {
        // OBJETOS SIMPLES
        // VALOR TEXTO
        DEFAULT = 0,
        STRING,

        // VALORES PRIMITIVOS
        UNSIGNEDLONG,
        SHORT,
        INT,
        INTEGER,
        DECIMAL,
        LONG,
        DATE,

        // OBJETOS COMPLEXOS
        OBJETO,
        OBJETO_IMPORTADO,
        OBJETO_ARRAY,
        EXTENSION,
    }
}
