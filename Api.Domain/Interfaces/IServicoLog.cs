using Api.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IServicoLog : IDisposable
    {
        bool IsDebug { get; set; }

        void CriarLog(string servico, string data, TipoLog tipo);
        
        void CriarLog(Exception ex, string mensagem);

        Task Save();
    }
}
