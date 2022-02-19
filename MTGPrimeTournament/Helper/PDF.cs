using HandlebarsDotNet;
using System;
using System.Globalization;
using System.IO;

namespace MTGPrimeTournament.Helper
{
    public class PDF
    {
        private Func<object, string> template { get; }
        private byte[] pdfBytes { get; set; }

        public PDF(string templatePath)
        {
            var rawHtml = File.ReadAllText(templatePath);
            this.template = Handlebars.Compile(rawHtml);

            /*Handlebars.RegisterHelper("card_status", (TextWriter output, dynamic context, object[] arguments) => {
                var status = (string)arguments[0];
                var cardStatus = CardStatus.FromId(status) ?? CardStatus.ToPrint;
                output.WriteSafeString(cardStatus.label);
            });

            Handlebars.RegisterHelper("date", (TextWriter output, dynamic context, object[] arguments) => {
                var ci = new CultureInfo("fr-FR");
                var date = (DateTime)arguments[0];
                output.WriteSafeString(date.ToString("d", ci));
            });*/
        }

        public void Generate(object data)
        {
            var compiledHtml = this.template(data);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            this.pdfBytes = htmlToPdf.GeneratePdf(compiledHtml);
        }

        public void SaveAs(string location)
        {
            using (var fs = new FileStream(location, FileMode.Create, FileAccess.Write))
            {
                fs.Write(this.pdfBytes, 0, this.pdfBytes.Length);
            }
        }
    }
}
