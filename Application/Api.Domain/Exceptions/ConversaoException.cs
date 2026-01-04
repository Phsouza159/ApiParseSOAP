using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Exceptions
{
    internal class ConversaoException : Exception
    {
        public ConversaoException()
        {
        }

        public ConversaoException(string mensagem)
            : base(mensagem) 
        {
        }
    }
}
