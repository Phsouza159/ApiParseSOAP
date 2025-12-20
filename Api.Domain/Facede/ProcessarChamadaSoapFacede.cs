using Api.Domain.Configuracao;
using Api.Domain.Interfaces;
using Api.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

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

                    var document = ServicoArquivosWsdl.TransformarXml(xmlContrato);
                    var conteudo = ServicoArquivosWsdl.TransformarXml(xmlConteudo);

                    var schema = ServicoArquivosWsdl.CarregarXml(document, conteudo);

                    // VALIR SE SERVICO ESTA CONFIGURADO
                    if (!servicoConfiguracao.Contratos.Any(e => e.Servico.ToLower().Equals(schema.NomeServico.ToLower())))
                        throw new ArgumentException($"Serviço não registrado: '{schema.NomeServico}'");

                    schema.Servico = servicoConfiguracao;
                    return schema;
                }
            }

            return new Schema() { IsVazio = true };
        }

        public async Task EnviarProcessamento(Schema schema, IServicoLog servicoLog)
        {
            await this.ServicoWeb.Enviar(schema, servicoLog);
        }
    }
}
