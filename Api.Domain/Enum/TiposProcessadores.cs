using System.ComponentModel;

namespace Api.Domain.Enum
{
    public enum TiposProcessadores
    {
        DEFAULT = 0,
        unsignedLong,
        STRING,
        SHORT,

        // OBJETOS COMPLEXOS
        OBJETO,
        OBJETO_IMPORTADO,
        EXTENSION,
    }
}
