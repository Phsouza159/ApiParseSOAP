using Api.Domain.Api.Domain;
using Api.Domain.Interfaces.Repository;
using Newtonsoft.Json;
using ProcessamentoLog.Domain.Interface;
using System.IO;

namespace ProcessamentoLog.Domain
{
    internal class ProcessoLog : IProcessoLog
    {
        public ProcessoLog(IRepositorioData repositorioData)
        {
            this.PastaLog = string.Empty;
            this.CaminhoData = string.Empty;
            this.Arquvos = new List<string>();
            this.RegistrosLogs = new List<RegistroLog>();
            this.RepositorioData = repositorioData;
        }

        public string PastaLog { get; set; }

        public string CaminhoData { get; set; }

        private List<string> Arquvos { get; set; }

        private List<RegistroLog> RegistrosLogs { get; set; }

        public IRepositorioData RepositorioData { get; }


        public void Executar()
        {
            this.RepositorioData.ConfgurarCaminhoData(this.CaminhoData);

            this.CarregarArquivosLog();
            this.CarregarLog();
            this.ProcessarRegistrosLog();
            this.LimparRegistros();
        }

        private void LimparRegistros()
        {
            foreach (var arquivo in this.Arquvos)
            {
                FileInfo fileInfo = new FileInfo(arquivo);

                if (fileInfo.Exists && fileInfo.Extension == ".temp")
                {
                    Console.WriteLine($" * [DELETANDO] - Arquivo log: {fileInfo.Name}");
                    fileInfo.Delete();
                }
            }
        }

        private void ProcessarRegistrosLog()
        {
            List<Guid> arquivosFalha = new List<Guid>();

            foreach (var registro in this.RegistrosLogs)
            {
                bool sucessoGravacao = this.RepositorioData.AdicionarRegistroLog(registro);
                if (!sucessoGravacao)
                    arquivosFalha.Add(registro.ID);
            }
        }

        private void CarregarLog()
        {
            foreach (var arquivo in this.Arquvos)
            {
                FileInfo file = new FileInfo(arquivo);
                if (!file.Exists && file.Extension != ".temp") continue;

                Console.WriteLine($" * [CARREGANDO] - Arquivo log: {file.Name}");

                var registros = JsonConvert.DeserializeObject<List<RegistroLog>>(File.ReadAllText(arquivo));
                
                if(registros != null)
                    this.RegistrosLogs.AddRange(registros);
            }
        }

        private void CarregarArquivosLog()
        {
            var arquivos = Directory.GetFiles(this.PastaLog);
            var dataFiltro = DateTime.Now.AddMinutes(-1);

            foreach (var arquivo in arquivos)
            {
                FileInfo file = new FileInfo(arquivo);

                if(file.CreationTime <= dataFiltro && file.Extension == ".temp")
                {
                    this.Arquvos.Add(arquivo);
                }
            }
        }


        #region DISPONSE

        public bool IsDiponse { get; set; }

        public void Dispose()
        {
            if(!this.IsDiponse)
            {
                this.IsDiponse = true;
                this.RepositorioData.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
