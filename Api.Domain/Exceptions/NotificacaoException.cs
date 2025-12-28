using Api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Exceptions
{
    internal class NotificacaoException : Exception
    {
        private NotificacaoException(string mensagem)
            : base(mensagem)
        {
        }

        public static NotificacaoException Criar(INotificacoes notificacoes, string erroMensagem)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(erroMensagem);
            stringBuilder.Append($"{Environment.NewLine}Lista Menagem:");

            foreach (var textoMensagem in notificacoes.Notificacoes.Mensagens)
                stringBuilder.Append($"{Environment.NewLine}\t{textoMensagem}");

            return new NotificacaoException(stringBuilder.ToString());
        }
    }
}
