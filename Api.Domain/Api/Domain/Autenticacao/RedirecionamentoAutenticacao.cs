using Api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Api.Domain.Autenticacao
{
    public class RedirecionamentoAutenticacao : IAutenticacao
    {
        public RedirecionamentoAutenticacao(string token)
        {
            this.TokenAutenticacao = token;
        }
        public string TokenAutenticacao { get; set; }

        public void CarregarAutenticacao(HttpClient client, IServicoLog servicoLog)
        {
            if (!string.IsNullOrEmpty(this.TokenAutenticacao) && this.TokenAutenticacao.Contains(' '))
            {
                servicoLog.CriarLog("", "Carregando token de autenticação", Enum.TipoLog.INFO);

                string[] parametros = this.TokenAutenticacao.Split(' ');
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parametros[0], parametros[1]);
            }
        }
    }
}
