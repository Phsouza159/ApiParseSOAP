using Api.Domain.Configuracao;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Facede
{
    public class ProcessarChamadaSoapFacede : IProcessarChamadaSoapFacede
    {
        public ProcessarChamadaSoapFacede(IServicoWeb servicoWeb)
        {
            ServicoWeb = servicoWeb;
        }

        public IServicoWeb ServicoWeb { get; }

        public Schema CarregarDadosChamadaSoap(string servico, string xmlConteudo)
        {
            Servicos? servicoConfiguracao = ServicoArquivosWsdl.RecuperarServico(servico);

            if (servicoConfiguracao != null)
            {
                string nomeTratado = ServicoArquivosWsdl.ResoverNomeArquivo(servicoConfiguracao.Nome);

                if (servicoConfiguracao.ConteudoArquivos.ContainsKey(nomeTratado))
                {
                    byte[] data = servicoConfiguracao.ConteudoArquivos[nomeTratado];
                    string xmlContrato = Encoding.UTF8.GetString(data);

                    var schema = ServicoArquivosWsdl.CarregarXml(
                              ServicoArquivosWsdl.TransformarXml(xmlContrato)
                            , ServicoArquivosWsdl.TransformarXml(xmlConteudo));

                    schema.Servico = servicoConfiguracao;
                    return schema;
                }
            }

            return new Schema() { IsVazio = true };
        }

        public async Task EnviarProcessamento(Schema schema)
        {
            await this.ServicoWeb.Enviar(schema);
        }
    }
}
