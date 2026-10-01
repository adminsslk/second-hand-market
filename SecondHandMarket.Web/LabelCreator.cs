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
            PdfPage page = new PdfPage();
            page.Width = XUnit.FromMillimeter(90.3);
            page.Height = XUnit.FromMillimeter(29);
            doc.Pages.Add(page);

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                string itemText = item.Id.ToString() + " | " + item.Description;
                gfx.DrawString(itemText, font, XBrushes.Black, new XRect(0, 0, page.Width, 15), XStringFormats.TopLeft);

                gfx.DrawLine(pen, new XPoint(0, 60), new XPoint(400, 60));

                string itemPrice = item.Price.ToString() + " kr";
                gfx.DrawString(itemPrice, font, XBrushes.Black, new XRect(0, 65, page.Width, 15), XStringFormats.TopLeft);

                CreateBarcode(item.Id, gfx);
            }
        }


        // Brother QL-700 skriver ut med 300 dpi. Smalaste stapeln (modulen) är exakt 5 punkter
        // så att alla staplar blir lika breda, oavsett hur långt varunumret är.
        private const double BarcodeModuleWidth = 5 * 72.0 / 300;          // 1,2 pt ≈ 0,42 mm
        private const double BarcodeQuietZone = 10 * BarcodeModuleWidth;   // vit marginal som Code 128 kräver
        private const double PrinterMargin = 1.5 * 72 / 25.4;              // ej utskrivbar kant, 1,5 mm
        private const double BarcodeTop = 18;
        private const double BarcodeHeight = 40;

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