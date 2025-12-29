using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Interfaces
{
    public interface IAutenticacao
    {
        void CarregarAutenticacao(HttpClient client, IServicoLog servicoLog);
    }
}
