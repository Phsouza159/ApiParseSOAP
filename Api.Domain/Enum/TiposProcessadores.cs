using System.ComponentModel;

namespace Api.Domain.Enum
{
    public enum TiposProcessadores
    {
        DEFAULT = 0,
        UNSIGNEDLONG,
        STRING,
        SHORT,
        INTEGER,
        LONG,

        // OBJETOS COMPLEXOS
        OBJETO,
        OBJETO_IMPORTADO,
        OBJETO_ARRAY,
        EXTENSION,
    }
}
