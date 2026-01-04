using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessamentoLog.Domain.Interface
{
    internal interface IProcessoLog : IDisposable
    {
        string PastaLog { get; set; }
        string CaminhoData { get; set; }

        void Executar();
    }
}
