using Api.Domain.Api.Domain;
using Api.Domain.Conversor;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using System.Xml;

namespace Api.Domain.Api
{
    public class Elemento : ObjetoBase
    {
        public Elemento()
        {
            Nome = string.Empty;
            Prefixo = string.Empty;
            Valor = string.Empty;
            ElementosFilhos = [];
            Processador = new DadosProcessamento();
        }

        public string Nome { get; set; }

        public string Prefixo { get; set; }

        public object? Valor { get; set; }

        public XmlNodeType Tipo { get; set; }

        public List<Elemento> ElementosFilhos { get; set; }

        public DadosProcessamento Processador { get; set; }

        #region COPIAR

        internal void Copiar(Elemento item)
        {
            item.Nome = Nome;
            item.Valor = Valor;
            item.Processador = Processador;
            item.Tipo = Tipo;
        }

        #endregion

        #region RECUPERAR NOTIFICACOES

        /// <summary>
        /// Recuperar Todas notificacoes dos itens filhos
        /// </summary>
        internal Notificacoes RecuperarNotificacoesItensFilhos()
        {
            foreach (var item in ElementosFilhos)
                Notificacoes.AdicionarMensagem(item.RecuperarNotificacoesItensFilhos());

            return Notificacoes;
        }

        #endregion
    }
}
