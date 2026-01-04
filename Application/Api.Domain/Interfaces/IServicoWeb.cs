using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IServicoWeb
    {
        Task Enviar(Schema schema, ObjectValues.EnvelopeEnvio envelope, IServicoLog servicoLog);
    }
}
