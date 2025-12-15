
using Microsoft.AspNetCore.Mvc;

namespace ApiParseSOAP.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
        }

        internal string RecuperarCorpoChamada()
        {
            using var reader = new StreamReader(Request.Body);
            return reader.ReadToEnd();
        }
    }
}
