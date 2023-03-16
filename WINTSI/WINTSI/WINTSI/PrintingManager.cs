
using System;

namespace GenesisCreations.PharmaTech
{
    using System.Drawing;
    using System.Drawing.Printing;

    public static class PrintingManager
    {
        static PaymentResponse PaymentResponse;
        
        static System.Drawing.Font drawFontArial12Bold = new("Arial", 12, System.Drawing.FontStyle.Bold);
        static System.Drawing.Font drawFontArial10Regular = new("Arial", 10, System.Drawing.FontStyle.Regular);
        static SolidBrush drawBrush = new(System.Drawing.Color.Black);
        
        static StringFormat DrawFormatCenter = new() { Alignment = StringAlignment.Center };
        static StringFormat DrawFormatLeft = new() { Alignment = StringAlignment.Near };
        static StringFormat DrawFormatRight = new() { Alignment = StringAlignment.Far };

        static float CurrentX;
        static float CurrentY;
        static float Width;
        static float Height;
        
        public static void PrintReceiptForTransaction(PaymentResponse paymentResponse)
        {
            Console.WriteLine("Attempting print.");
            PaymentResponse = paymentResponse;
            
            var recordDoc = new PrintDocument();
            recordDoc.DocumentName = "Customer Receipt";
            recordDoc.PrintPage += PrintReceiptPage;
            recordDoc.PrintController = new StandardPrintController();
            Console.WriteLine("Document initialized.");
            recordDoc.PrinterSettings = new PrinterSettings
            {
                /*PrintToFile = true,
                PrintFileName = @"C:\Test.xps"*/
                //PrinterName = "EPSON TM-T88VII Receipt",
                PrintToFile = false
            };
            Console.WriteLine("Initialized printer settings.");
            recordDoc.EndPrint += OnPrintEnded;
            Console.WriteLine("Printing.");
            recordDoc.Print();
            Console.WriteLine("Disposing.");
            recordDoc.Dispose();
        }

        private static void OnPrintEnded(object sender, PrintEventArgs e)
        {
            Console.WriteLine("A receipt has been printed successfully.");
            WINTSI.WebSocket.Client.SocketClient.Send("{\"status\":200, \"message\":\"success\", \"details\":\"Receipt printed successfully.\"}");
        }

        static void PrintReceiptPage(object sender, PrintPageEventArgs e)
        {
            Console.WriteLine("Printing page.");
            CurrentX = 10;
            CurrentY = 5;
            Width = 270.0f;
            Height = 0f;

            PrintHeader("PureHealth Pharmacy", ref e);
            PrintLine("This is a remote printing test.", ref e);
            PrintLine($"Message received: {PaymentResponse.method}.", ref e);
        }

        static void PrintLine(string line, ref PrintPageEventArgs e, StringFormat format = null, System.Drawing.Font font = null)
        {
            Console.WriteLine("Printing line.");
            format ??= DrawFormatLeft;
            font ??= drawFontArial10Regular;
            e.Graphics.DrawString(line, font, drawBrush, new RectangleF(CurrentX, CurrentY, Width, Height), format);
            CurrentY += e.Graphics.MeasureString(line, font).Height;
        }
        
        static void PrintHeader(string line, ref PrintPageEventArgs e) => PrintLine(line, ref e, DrawFormatCenter, drawFontArial12Bold);
    }
}
