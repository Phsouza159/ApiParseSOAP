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
                ID                 = Guid.Parse(reader["CODIGO_TICKET"].ToString()),
                DescricaoTipoLog  = reader["DESCRICAO"].ToString(),
                Conteudo          = reader["REGISTRO"].ToString(),
                Data              = DateTime.Parse(reader["DTH_REGISTRO"].ToString())
            };
        }
    }
}


//SELECT
//      LOG.ID
//	, LOG.CODIGO_TICKET
//	, TIP.DESCRICAO
//	, LOG.REGISTRO
//	, LOG.DTH_REGISTRO
//FROM REGISTRO_LOG AS LOG
//	LEFT JOIN TIPO_LOG AS TIP ON TIP.COD_TIPO = LOG.COD_TIPO_LOG ;
