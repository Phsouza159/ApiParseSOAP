using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Api.Domain
{
    public abstract class ObjetoBase : INotificacoes, IDisposable
    {
        public ObjetoBase()
        {
            this.Notificacoes = new Notificacoes();
        }

        public Notificacoes Notificacoes { get; }

        #region DISPONSE

        internal bool IsDisponse { get; set; }

        public void Dispose()
        {
            if (!IsDisponse)
            {
                IsDisponse = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
