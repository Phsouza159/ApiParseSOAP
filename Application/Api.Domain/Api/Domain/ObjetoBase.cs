using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Api.Domain.Api.Domain
{
    public abstract class ObjetoBase : INotificacoes, IDisposable
    {
        public ObjetoBase()
        {
            this.Notificacoes = new Notificacoes();
        }

        [System.Text.Json.Serialization.JsonIgnore]
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
