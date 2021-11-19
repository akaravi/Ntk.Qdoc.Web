using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ntk.Qdoc.Web.Blazor.Helper
{
    public static class ChatHelper
    {
        public static string RandomString(int length)
        {
            Random random = new Random();
            //const string chars = "ABCDEFGHKMNPRSTUVWXYZ23456789";
            const string chars = "1234567890";
            var retOut= new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            if (retOut.Substring(0,1) == "0")
            {
                retOut = RandomString(length);
            }

            return retOut;
        }
    }
}
