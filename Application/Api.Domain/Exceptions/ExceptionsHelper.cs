using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Exceptions
{
    public static class ExceptionsHelper
    {
        public static string RecuperarTraceErroTratado(this Exception exception)
        {
            return @$"
Tipo de Erro: {exception.GetType().Name};
#----------
Mensagem Erro: 
{exception.Message};
#----------
Stack Trace: 
    {exception.StackTrace}
#----------
Stack Trace Mensagem: 
{exception.InnerException?.Message}
#----------
            ";
        }
    }
}
