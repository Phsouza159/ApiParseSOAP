using Api.Domain.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessamentoLog.Domain.Interface
{
    internal interface IServicoData : IDisposable
    {
        string CaminhoArquivoData { get; set; }

        bool Adicionar(RegistroLog registro);
    }
}
