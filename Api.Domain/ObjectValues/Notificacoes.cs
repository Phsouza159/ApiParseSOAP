using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.ObjectValues
{
    public class Notificacoes 
    {
        public Notificacoes()
        {
            this.Mensagens = new List<string>();
        }

        public bool IsValido { get => this.Mensagens.Count == 0; }

        public List<string> Mensagens { get; private set; }

        public void AdicionarMensagem(string mensagem)
        {
            this.Mensagens.Add(mensagem);
        }
        public void AdicionarMensagem(Notificacoes registro)
        {
            this.Mensagens.AddRange(registro.Mensagens);
        }

        public void AdicionarMensagem(IEnumerable<Notificacoes> registros)
        {
            foreach (var registro in registros)
            {
                this.AdicionarMensagem(registro);
            }
        }


    }
}
