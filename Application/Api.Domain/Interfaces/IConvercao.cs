using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IConvercao : INotificacoes, IDisposable
    {
        List<Element> ConverterContrato(Schema schema);
    }
}
