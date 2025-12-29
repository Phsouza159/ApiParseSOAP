using Api.Domain;
using Api.Domain.Api.Domain.Autenticacao;
using Api.Domain.Configuracao;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using System.Text;

namespace Api.Application.Facede
{
    public class ProcessarChamadaSoapFacede : IProcessarChamadaSoapFacede
    {
        public ProcessarChamadaSoapFacede(IServicoWeb servicoWeb)
        {
            ServicoWeb = servicoWeb;
        }

        public IServicoWeb ServicoWeb { get; }

        public Schema CarregarDadosChamadaSoap(string servico, string xmlConteudo, string autenticacao)
        {
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if (servicoConfiguracao != null)
            {
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servicoConfiguracao.Nome);

                if (servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
                {
                    byte[] data = servicoConfiguracao.ConteudoArquivos[nomeTratado];
                    string xmlContrato = Encoding.UTF8.GetString(data);

                    var document = ServicoArquivosWsdl.TransformarXml(xmlContrato);
                    var conteudo = ServicoArquivosWsdl.TransformarXml(xmlConteudo);

                    var schema = ServicoArquivosWsdl.CarregarXml(document, conteudo);

                    // VALIR SE SERVICO ESTA CONFIGURADO
                    if (!servicoConfiguracao.Contratos.Any(e => e.Servico.ToLower().Equals(schema.NomeServico.ToLower())))
                        throw new ArgumentException($"Serviço não registrado: '{schema.NomeServico}'");

                    schema.Servico = servicoConfiguracao;
                    
                    this.CarregarDadosAutenticacao(schema, autenticacao);

                    return schema;
                }
            }

            return new Schema() { IsVazio = true };
        }

        private void CarregarDadosAutenticacao(Schema schema, string autenticacao)
        {
            var contrato = schema.GetContrato();

            if(contrato != null && !string.IsNullOrEmpty(autenticacao) && !string.IsNullOrEmpty(contrato.Autenticacao))
            {
                schema.Autenticacao = contrato.Autenticacao switch
                {
                    "REDIRECIONAR_AUTENTICACAO" => new RedirecionamentoAutenticacao(autenticacao),
                    // DEFAULT
                    _ => throw new ArgumentException($"Tipo autenticação não suportado: {contrato.Autenticacao}"),
                };
            }
        }
    }
}
