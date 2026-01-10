using Api.Domain.Api.Domain;
using Api.Domain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces.Facede
{
    public interface IRegistroLogFacede
    {
        ResponseData<List<RegistroLog>> RecuperarRegistroLog(DateTime dataInicio, DateTime dataFim);
    }
}
