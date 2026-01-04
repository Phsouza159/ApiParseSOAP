using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IServicoWebFacede
    {

        Task<EnvelopeEnvio> EnviarProcessamento(Schema schema, IServicoLog servicoLog);

    }
}
