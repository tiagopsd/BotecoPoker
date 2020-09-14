using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Infra.Impressora
{
    public class ImpressaoBase
    {
        public const string Epson = "EPSON TM-T88V Receipt";
        public const string CS = "CIS PR 3000";

        public void ImprimeUmaVez(PrintPageEventHandler eventoEpson, PrintPageEventHandler eventoCIS, string nomeImpressora)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = "Cupom";

            if (nomeImpressora.ToLower() == "epson")
            {
                printDoc.PrinterSettings.PrinterName = Epson;
                printDoc.PrintPage += eventoEpson;
            }
            else if (nomeImpressora.ToLower() == "cis")
            {
                //printDoc.OriginAtMargins = true;
                printDoc.PrinterSettings.PrinterName = CS;
                printDoc.PrintPage += eventoCIS;
            }
            else if (!printDoc.PrinterSettings.IsValid)
                throw new Exception("Não foi possível localizar a impressora");

            printDoc.Print();
        }

        public void ImprimeDuasVezes(PrintPageEventHandler evento)
        {
            PrintDocument printDoc = new PrintDocument();
            //printDoc.PrinterSettings.PrinterName = "Epson";
            printDoc.DocumentName = "Cupom";

            if (!printDoc.PrinterSettings.IsValid)
                throw new Exception("Não foi possível localizar a impressora");

            printDoc.PrintPage += evento;

            printDoc.Print();
            printDoc.Print();
        }
    }
}
