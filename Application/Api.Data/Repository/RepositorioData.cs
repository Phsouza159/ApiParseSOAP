using Api.Domain.Api.Domain;
using Api.Domain.Interfaces.Repository;
using Microsoft.Data.Sqlite;

namespace Api.Data.Repository
{
    public class RepositorioData : IRepositorioData
    {
        public RepositorioData()
        {
            this.CaminhoArquivoData = string.Empty;
        }

        public string CaminhoArquivoData { get; private set; }

        public void ConfgurarCaminhoData(string caminho)
        {
            this.CaminhoArquivoData = caminho;
            if (!File.Exists(this.CaminhoArquivoData))
                throw new ArgumentException($"Caminho arquivo DB não localizado: {caminho}");
        }

        #region ADD REGISTRO LOG

        public bool AdicionarRegistroLog(RegistroLog registro)
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

        #endregion

        public bool IsDisponse { get; set; }

        public void Dispose()
        {
            if(!this.IsDisponse)
            {
                this.IsDisponse = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
