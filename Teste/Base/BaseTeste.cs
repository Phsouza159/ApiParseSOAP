using Api.Domain;
using Api.Domain.Configuracao;
using Api.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Base
{
    public abstract class BaseTeste
    {
        protected string PathArquivosTeste = "C:\\_git\\ApiParseSOAP\\Api.Soap\\Servicos\\Contratos";

        protected BaseTeste()
        {
        }

        internal Servicos CarregarArquivoSchema(string nomeServico)
        {
            string diretorio = Path.Combine(this.PathArquivosTeste, nomeServico);

            if (!Directory.Exists(diretorio))
                throw new ArgumentException("Diretorio para Teste não localizado: " + diretorio);

            var servico = ServicoArquivosWsdl.CriarRegistroServico(diretorio);

            Assert.IsNotNull(servico, $"Falha ao criar o registro do Servico: {diretorio}");
            ServicoArquivosWsdl.Configuracacoes.Add(servico);

            return servico;
        }


        //interface IProcessarChamadaSoapFacede RecuperarFacedeSaop()
        //{
        //    return new ProcessarChamadaSoapFacede();
        //}
    }
}
