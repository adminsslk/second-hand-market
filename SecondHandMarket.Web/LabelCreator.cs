using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using SecondHandMarket.Database;
using PdfSharp.Drawing.BarCodes;
using BarcodeLib;
using System.Drawing;

namespace SecondHandMarket.Web
{
    public class LabelCreator
    {
        public string CreateLabelPdf(string phone)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            
            User user = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();
            if (user == null)
                return null;
            int activeYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);

            return SaveLabels(user.Items.Where(i => i.Year == activeYear), phone);
        }

        public string CreateLabelPdf(string phone, int itemId)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();

            Item item = ctx.Items.Find(itemId);
            if (item == null)
                return null;

            return SaveLabels(new[] { item }, phone);
        }

        private string SaveLabels(IEnumerable<Item> items, string phone)
        {
            PdfDocument doc = new PdfDocument();
            XFont font = new XFont("Verdana", 11, XFontStyle.Bold);
            XPen pen = new XPen(XColor.FromName("Black"), 1);

            foreach (Item item in items)
            {
                for (int i = 0; i < item.NumberOfLabels; i++)
                    DrawLabel(doc, item, font, pen);
            }

            if (doc.PageCount == 0)
                return null;

            string filename = "etiketter-" + phone + ".pdf";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/") + filename;
            doc.Save(path);

            return filename;
        }

        private void DrawLabel(PdfDocument doc, Item item, XFont font, XPen pen)
        {
            // Samma mått som drivrutinens "29mm x 90mm" (DK-11201), som är 89,8 mm lång. Med etikettens
            // nominella 90,3 mm blir sidan för lång och skrivaren skalar om, klipper eller matar fram en extra etikett.
            PdfPage page = new PdfPage();
            page.Width = XUnit.FromMillimeter(89.8);
            page.Height = XUnit.FromMillimeter(29);
            doc.Pages.Add(page);

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                double right = page.Width - PrinterMargin;

                // Översta raden krymps om den inte får plats, t.ex. med sexsiffriga varunummer och långa beskrivningar
                string itemText = item.Id.ToString() + " | " + item.Description;
                XFont textFont = FitFont(gfx, itemText, font, right - LabelMarginLeft);
                gfx.DrawString(itemText, textFont, XBrushes.Black, new XRect(LabelMarginLeft, LabelMarginTop, right - LabelMarginLeft, 15), XStringFormats.TopLeft);

                gfx.DrawLine(pen, new XPoint(LabelMarginLeft, LineTop), new XPoint(right, LineTop));

                string itemPrice = item.Price.ToString() + " kr";
                gfx.DrawString(itemPrice, font, XBrushes.Black, new XRect(LabelMarginLeft, PriceTop, right - LabelMarginLeft, 15), XStringFormats.TopLeft);

                CreateBarcode(item.Id, gfx);
            }
        }

        private static XFont FitFont(XGraphics gfx, string text, XFont font, double maxWidth)
        {
            XFont fitted = font;
            for (double size = font.Size; size >= MinFontSize && gfx.MeasureString(text, fitted).Width > maxWidth; size -= 0.5)
                fitted = new XFont(font.Name, size, font.Style);
            return fitted;
        }

        // Skrivaren kan inte skriva ända ut i kanten, så allt hålls innanför marginalerna (i punkter, 72 per tum)
        private const double LabelMarginLeft = 3 * 72 / 25.4;               // 3 mm
        private const double LabelMarginTop = 2 * 72 / 25.4;                // 2 mm
        private const double LineTop = 60;
        private const double PriceTop = 63;                                 // slutar ca 2 mm från nederkanten
        private const double MinFontSize = 7;


        // Brother QL-700 skriver ut med 300 dpi. Smalaste stapeln (modulen) är exakt 5 punkter
        // så att alla staplar blir lika breda, oavsett hur långt varunumret är.
        private const double BarcodeModuleWidth = 5 * 72.0 / 300;          // 1,2 pt ≈ 0,42 mm
        private const double BarcodeQuietZone = 10 * BarcodeModuleWidth;   // vit marginal som Code 128 kräver
        private const double PrinterMargin = 1.5 * 72 / 25.4;              // ej utskrivbar kant, 1,5 mm
        private const double BarcodeTop = 20;                               // under översta raden
        private const double BarcodeHeight = 38;                            // slutar ovanför linjen

        private void CreateBarcode(int id, XGraphics gfx)
        {
            try
            {
                string pattern;
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                using (Image img = b.Encode(BarcodeLib.TYPE.CODE128, id.ToString()))
                {
                    pattern = b.EncodedValue;
                }

                // Högerjustera med vit marginal mot kanten, och rita varje stapel som en rektangel
                double left = gfx.PageSize.Width - PrinterMargin - BarcodeQuietZone - pattern.Length * BarcodeModuleWidth;
                int i = 0;
                while (i < pattern.Length)
                {
                    if (pattern[i] != '1')
                    {
                        i++;
                        continue;
                    }

                    int start = i;
                    while (i < pattern.Length && pattern[i] == '1')
                        i++;

                    gfx.DrawRectangle(XBrushes.Black, left + start * BarcodeModuleWidth, BarcodeTop, (i - start) * BarcodeModuleWidth, BarcodeHeight);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

    }
}