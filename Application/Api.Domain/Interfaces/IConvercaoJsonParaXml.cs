using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Api.Domain.Api;

namespace Api.Domain.Interfaces
{
    public interface IConvercaoJsonParaXml : IConvercao
    {
        string ConverterParaXml(Schema schema);
    }
}
