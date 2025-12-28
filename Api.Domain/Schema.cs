using Api.Domain.Configuracao;
using System.Net;
using System.Text.Json.Nodes;
using System.Xml;

namespace Api.Domain
{
    public class Schema : IDisposable
    {
        public XmlDocument Documento { get; set; }

        public XmlNode Corpo { get; private set; }

        public XmlDocument Contrato { get; set; }

        public List<XmlNode> XmlNodes { get; set; }
        
        public string NomeServico { get; private set; }

        public IEnumerable<Element> Element { get; set; }

        public string Resultado { get; set; }
        
        public HttpStatusCode Status { get; set; }
        
        public Servicos Servico { get; set; }

        public bool IsVazio { get; set; }

        public bool IsElementoEntrada { get; set; }

        internal void Carregar()
        {
            var doc = this.Documento;
            var nodes = doc.ChildNodes.Cast<XmlNode>().ToList();

            var envelope = nodes.First(e => e.Name == "soapenv:Envelope");

            if (envelope is null) throw new ArgumentException("Nao localizado - Envelope");

            var body = envelope.ChildNodes.Cast<XmlNode>().FirstOrDefault(e => e.Name == "soapenv:Body");

            if (body is null) throw new ArgumentException("Nao localizado - Body");

            var servico = body.ChildNodes[0];

            if (servico is null) throw new ArgumentException("Nao localizado - Servico");

            this.XmlNodes = servico.ChildNodes.Cast<XmlNode>().ToList();
            this.Corpo = body;
            this.NomeServico = servico.LocalName;
        }

        internal bool IsDisponse { get; set; }

        public void Dispose()
        {
            if(!this.IsDisponse)
            {
                this.IsDisponse = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
