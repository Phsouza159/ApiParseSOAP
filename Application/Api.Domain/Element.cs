
using Api.Domain.Conversor;
using Api.Domain.Enum;
using Api.Domain.Helper;
using Api.Domain.Interfaces;
using Api.Domain.ObjectValues;
using System.Text.Json.Nodes;
using System.Xml;

namespace Api.Domain
{
    public class Element : INotificacoes, IDisposable
    {
        public Element()
        {
            this.Nome = string.Empty;
            this.Prefixo = string.Empty;
            this.Valor = string.Empty;
            this.No = [];
            this.Processador = new DadosProcessamento();
            this.Notificacoes = new Notificacoes();
        }

        public string Nome { get; set; }

        public string Prefixo { get; set; }

        public object? Valor { get; set; }

        public XmlNodeType Tipo { get; set; }

        public List<Element> No { get; set; }

        /// <summary>
        /// Definicao de registro como Propriedade
        /// </summary>
        public bool IsPropriedade { get; set; }

        /// <summary>
        /// Definicao para registro obrigatorio
        /// </summary>
        public bool IsObrigatorio { get; set; }

        public DadosProcessamento Processador { get; set; }

        public Notificacoes Notificacoes { get; }

        #region COPIAR

        internal void Copiar(Element item)
        {
            item.Nome = this.Nome;
            item.Valor = this.Valor;
            item.Processador = this.Processador;
            item.IsPropriedade = this.IsPropriedade;
            item.Tipo = this.Tipo;
        }

        #endregion

        #region RECUPERAR NOTIFICACOES

        /// <summary>
        /// Recuperar Todas notificacoes dos itens filhos
        /// </summary>
        internal Notificacoes RecuperarNotificacoesItensFilhos()
        {
            foreach (var item in this.No)
                this.Notificacoes.AdicionarMensagem(item.RecuperarNotificacoesItensFilhos());

            return this.Notificacoes;
        }

        #endregion

        #region DISPONSE

        internal bool IsDisponse { get; set; }

        public void Dispose()
        {
            if (!this.IsDisponse)
            {
                this.IsDisponse = true;
                this.No.Clear();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
