using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.ObjectValues
{
    public class ResponseData<T>
        where T : class
    {
        public string Mensagem { get; set; }

        public T Data { get; set; }
    }
}
