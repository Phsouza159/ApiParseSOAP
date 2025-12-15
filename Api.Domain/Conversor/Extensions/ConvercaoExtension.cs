using Api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Conversor.Extensions
{
    public static class ConvercaoExtension
    {
        public static string TratarCaseSensitive(this IConvercaoJsonParaXml elemento, string parametro)
        {
            //  string translate = "translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')";
            //  string xpath = "//xs:element[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='numbertowordsresponse']" +
            //  "//xs:element[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='numbertowordsresult']";

            return $"translate({parametro},'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')";
        }
    }
}
