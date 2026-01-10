using Api.Domain.Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Interfaces;
using System.Xml;

namespace Api.Domain.Api
{
    public class Schema : ObjetoBase
    {

        public Schema()
        {
        }

        public XmlDocument Documento { get; set; }

        public XmlNode Corpo { get; private set; }

        public XmlDocument Contrato { get; set; }

        public List<XmlNode> XmlNodes { get; set; }

        public string NomeServico { get; private set; }

        public string Resultado { get; set; }

        public Servicos Servico { get; set; }

        public bool IsVazio { get; set; }

        public bool IsElementoEntrada { get; set; }

        public IAutenticacao Autenticacao { get; set; }

        #region CARREGAR DADOS SCHEMA

        internal void Carregar()
        {
            var doc = Documento;
            var nodes = doc.ChildNodes.Cast<XmlNode>().ToList();

            var envelope = nodes.First(e => e.Name == "soapenv:Envelope");

            if (envelope is null) throw new ArgumentException("Nao localizado - Envelope");

            var body = envelope.ChildNodes.Cast<XmlNode>().FirstOrDefault(e => e.Name == "soapenv:Body");

            if (body is null) throw new ArgumentException("Nao localizado - Body");

            var servico = body.ChildNodes[0];

            if (servico is null) throw new ArgumentException("Nao localizado - Servico");

            XmlNodes = servico.ChildNodes.Cast<XmlNode>().ToList();
            Corpo = body;
            NomeServico = servico.LocalName;
        }

        #endregion

        #region GET CONTRATO

        public Contrato GetContrato()
        {
            return Servico.Contratos.First(e => e.Servico.ToLower().Equals(NomeServico.ToLower()));
        }

        #endregion
    }
}
