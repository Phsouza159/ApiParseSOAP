using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.Extensions;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Api.Domain.Configuracao;
using Api.Domain.Api.Domain;
using System.Text;
using Newtonsoft.Json;

namespace Api.Domain.Services
{
    public class ServicoLog : IServicoLog
    {

        public ServicoLog(IConfiguration configuration)
        {
            this.PathCaminhoPastaLog = configuration.GetPathLog();
            this.Ticket = Guid.NewGuid();
            this.Registros = new List<RegistroLog>();
        }
        internal Guid Ticket { get; }

        internal string PathCaminhoPastaLog { get; set; }

        internal List<RegistroLog> Registros { get; set; }

        public void CriarLog(string servico, string conteudo, TipoLog tipo)
        {
            this.Registros.Add(new RegistroLog()
            { 
                ID = this.Ticket,
                TipoLog = tipo,
                Servico = servico,
                Conteudo = conteudo,
                Data = DateTime.Now,
            });
        }

        public void CriarLog(Exception ex, string mensagem)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("Erro: {0} - Trace: {1}", ex.Message, ex.StackTrace));

            if(ex.InnerException != null)
                stringBuilder.Append(string.Format("{0} InnerException - Erro: {1} - Trace: {2}", Environment.NewLine, ex.InnerException.Message, ex.InnerException.StackTrace));

            this.Registros.Add(new RegistroLog()
            {
                ID = this.Ticket,
                Servico = "Erro servico",
                TipoLog = TipoLog.TRACE_ERRO,
                Conteudo = stringBuilder.ToString(),
                Data = DateTime.Now,
            });
        }

        #region DISPONSE

        internal bool IsDisponse { get; set; }

        public void Dispose()
        {
            if(!this.IsDisponse)
            {
                this.IsDisponse = true;
                this.Registros.Clear();
                GC.SuppressFinalize(this);
            }
        }

        internal bool IsSave { get; set; }

        public bool IsDebug { get; set; }

        public async Task Save()
        {
            string caminhoArquivo = Path.Combine(this.PathCaminhoPastaLog, $"{this.Ticket}.temp");

            if(File.Exists(caminhoArquivo))
                File.Delete(caminhoArquivo);

            await File.WriteAllTextAsync(
                  caminhoArquivo
                , JsonConvert.SerializeObject(this.Registros, Formatting.Indented)
                , UTF8Encoding.UTF8);

            this.IsSave = true;
        }

        #endregion
    }
}
