using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Domain.Api;

namespace Api.Domain.Interfaces
{
    public interface IServicoIntegracao
    {
        Task Enviar(Schema schema, ObjectValues.EnvelopeEnvio envelope, IServicoLog servicoLog);
    }
}
