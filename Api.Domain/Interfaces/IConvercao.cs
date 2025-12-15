using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IConvercao
    {
        List<Element> Converter(Schema schema);
    }
}
