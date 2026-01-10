using Api.Domain.Api.Domain;
using Api.Domain.Extensions;
using Api.Domain.Interfaces.Facede;
using Api.Domain.Interfaces.Repository;
using Api.Domain.ObjectValues;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Facede
{
    public class RegistroLogFacede : IRegistroLogFacede
    {
        public RegistroLogFacede(
              IConfiguration configuration
            , IRepositorioData repositorioData)
        {
            Configuration = configuration;
            RepositorioData = repositorioData;

            this.RepositorioData.ConfgurarCaminhoData(Configuration.GetPastaDatabase());
        }

        public IConfiguration Configuration { get; }

        public IRepositorioData RepositorioData { get; }

        public ResponseData<List<RegistroLog>> RecuperarRegistroLog(DateTime dataInicio, DateTime dataFim)
        {
            ResponseData<List<RegistroLog>> response = new ResponseData<List<RegistroLog>>();

            ParametroDatas parametro = new ParametroDatas()
            {
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            // VALIDAR DATA INICIO E DATA FIM
            parametro.ValidarPeriodo();

            response.Data = this.RepositorioData.RecuperarRegistros(parametro).ToList();
            return response;
        }
    }
}
