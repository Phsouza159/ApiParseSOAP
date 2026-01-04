using Api.Domain.Api.Domain;
using Microsoft.Data.Sqlite;
using ProcessamentoLog.Domain.Interface;

namespace ProcessamentoLog.Data
{
    public class ServicoData : IServicoData, IDisposable
    {
        public ServicoData()
        {
            this.CaminhoArquivoData = string.Empty;
        }

        public string CaminhoArquivoData { get; set; }

        public bool Adicionar(RegistroLog registro)
        {
            using var conn = new SqliteConnection($"Data Source={CaminhoArquivoData}");
            conn.Open();

            string query = @"INSERT INTO REGISTRO_LOG 
                     (CODIGO_TICKET, COD_TIPO_LOG, REGISTRO, DTH_REGISTRO)
                     VALUES (@id, @tipo, @conteudo, @data);"
            ;

            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", registro.ID);
            cmd.Parameters.AddWithValue("@tipo", (short)registro.TipoLog);
            cmd.Parameters.AddWithValue("@conteudo", registro.Conteudo);
            cmd.Parameters.AddWithValue("@data", registro.Data);

            return cmd.ExecuteNonQuery() == 1;
        }

        public bool IsDispose { get; set; }

        public void Dispose()
        {
            if(!this.IsDispose)
            {
                GC.SuppressFinalize(this);
                this.IsDispose = true;
            }
        }
    }
}
