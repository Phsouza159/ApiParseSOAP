using Api.Domain.Api.Domain;
using Api.Domain.Enum;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Data.Repository
{
    public static class Mapper
    {
        internal static RegistroLog ToRegistroLog(SqliteDataReader reader)
        {
            return new RegistroLog
            {
                ID       = Guid.Parse(reader["CODIGO_TICKET"].ToString()),
                TipoLog  = (TipoLog)Convert.ToInt32(reader["COD_TIPO_LOG"]),
                Conteudo = reader["REGISTRO"].ToString(),
                Data     = DateTime.Parse(reader["DTH_REGISTRO"].ToString())
            };
        }
    }
}
