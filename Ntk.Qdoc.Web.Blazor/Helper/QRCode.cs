using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using static QRCoder.PayloadGenerator;

namespace Ntk.Qdoc.Web.Blazor.Helper
{

    public static class QRCode
    {
        public enum ECCLevel
        {
            L = 0,
            M = 1,
            Q = 2,
            H = 3
        }

        public enum EciMode
        {
            Default = 0,
            Iso8859_1 = 3,
            Iso8859_2 = 4,
            Utf8 = 26
        }

        public static string getQRCode(string payloadString, ECCLevel eccLevel)
        {
            using (var generator = new QRCodeGenerator())
            {
                using (var data = generator.CreateQrCode(payloadString, (QRCodeGenerator.ECCLevel)eccLevel))
                {
                    var q64 = new Base64QRCode(data);
                    return q64.GetGraphic(20);
                }
            }
        }

        public static string UrlToQRCode(string urlTarget, ECCLevel eccLevel)
        {
            var generator = new Url(urlTarget);
            var payloadString = generator.ToString();
            return getQRCode(payloadString, eccLevel);
        }

        public static string VCARDToQRCode(VCARD model, ECCLevel eccLevel)
        {
            var payloadString = getPayLoad(model);
            return getQRCode(payloadString, eccLevel);
        }

        private static string getPayLoad(VCARD model)
        {
            if (model == null)
                return "";
            var textFormat =
                @"BEGIN:VCARD
VERSION:2.1";
            if (!string.IsNullOrEmpty(model.Name) || !string.IsNullOrEmpty(model.LastName))
                textFormat += @"
FN:{Name} {LName}";
            if (!string.IsNullOrEmpty(model.Title))
                textFormat += @"
TITLE:{Title}";
            if (!string.IsNullOrEmpty(model.CellPhone))
                textFormat += @"
TEL;CELL:{Phone-1}";
            if (!string.IsNullOrEmpty(model.Phone))
                textFormat += @"
TEL;WORK;VOICE:{Phone-2}";
            if (!string.IsNullOrEmpty(model.Email))
                textFormat += @"
EMAIL;HOME;INTERNET:{Email}";
            if (!string.IsNullOrEmpty(model.Website))
                textFormat += @"
URL:{Website}";
            if (!string.IsNullOrEmpty(model.Address))
                textFormat += @"
ADR:;;{Adress}";
            if (!string.IsNullOrEmpty(model.Organisation))
                textFormat += @"
ORG:{Organisation}";
            textFormat += @"
END:VCARD";

            var sb = new StringBuilder(textFormat);
            return sb
                .Replace("{Name}", model.Name)
                .Replace("{LName}", model.LastName)
                .Replace("{Title}", model.Title)
                .Replace("{Phone-1}", model.CellPhone)
                .Replace("{Phone-2}", model.Phone)
                .Replace("{Email}", model.Email)
                .Replace("{Website}", model.Website)
                .Replace("{Adress}", model.Address)
                .Replace("{Organisation}", model.Organisation)
                .ToString();
        }

        public class VCARD
        {
            public string Title { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public string CellPhone { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Website { get; set; }
            public string Address { get; set; }
            public string Organisation { get; set; }
        }
    }
}