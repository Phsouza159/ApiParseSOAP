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

        public XmlNode Corpo { get; set; }

        public XmlDocument Contrato { get; set; }

        public List<XmlNode> XmlNodes { get; set; }

        public string NomeServico { get; set; }

        public string Resultado { get; set; }

        public Servicos Servico { get; set; }

        public bool IsVazio { get; set; }

        public bool IsElementoEntrada { get; set; }


        public Contrato RecuperarContrato()
        {
            return this.Servico.Contratos.First(e => e.Servico.ToLower().Equals(this.NomeServico.ToLower()));
        }
    }
}
