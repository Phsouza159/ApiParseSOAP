using Api.Domain.Enum;
using Api.Domain.Interfaces;
using Api.Domain.Extensions;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Api.Domain.Configuracao;
using Api.Domain.Api.Domain;
using System.Text;

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
                Conteudo = conteudo,
                Data = DateTime.Now,
            });

            //string name = $"ID-{Ticket}-{Regex.Replace(servico, "[^A-Za-z0-9]", "")}-{tipo}.txt";
            //string path = Path.Combine(this.PathCaminhoPastaLog, name);
            //await File.WriteAllTextAsync(path, conteudo);
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

        public async Task Save()
        {
            string caminhoArquivo = Path.Combine(this.PathCaminhoPastaLog, $"{this.Ticket}.temp");
            StringBuilder stringBuilder = new();

            foreach (var registros in this.Registros)
                stringBuilder.AppendLine(registros.ToString());

            if(File.Exists(caminhoArquivo))
                File.Delete(caminhoArquivo);

            await File.WriteAllTextAsync(caminhoArquivo, stringBuilder.ToString(), UTF8Encoding.UTF8);

            this.IsSave = true;
        }

        #endregion
    }
}
