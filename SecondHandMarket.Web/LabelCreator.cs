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

            PdfDocument doc = new PdfDocument();

            foreach (Item item in user.Items.Where(i => i.Year == activeYear))
            {
                for (int i = 0; i < item.NumberOfLabels; i++)
                {
                    PdfPage page = new PdfPage();
                    page.Width = XUnit.FromMillimeter(90.3);
                    page.Height = XUnit.FromMillimeter(29);
                    doc.Pages.Add(page);

                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XFont font = new XFont("Verdana", 11, XFontStyle.Bold);
                    string itemText = item.Id.ToString() + " | " + item.Description;
                    gfx.DrawString(itemText, font, XBrushes.Black, new XRect(0, 0, page.Width, 15), XStringFormats.TopLeft);

                    XPen pen = new XPen(XColor.FromName("Black"), 1);
                    gfx.DrawLine(pen, new XPoint(0, 60), new XPoint(400, 60));

                    string itemPrice = item.Price.ToString() + " kr";
                    gfx.DrawString(itemPrice, font, XBrushes.Black, new XRect(0, 65, page.Width, 15), XStringFormats.TopLeft);

                    CreateBarcode(item.Id, gfx);
                }

            }
            
            string filename =  "etiketter-" + phone + ".pdf";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/") + filename;
            doc.Save(path);

            return filename;
        }

        public string CreateLabelPdf(string phone, int itemId)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();

            Item item = ctx.Items.Find(itemId);
            if (item == null)
                return null;

            PdfDocument doc = new PdfDocument();

            for (int i = 0; i < item.NumberOfLabels; i++)
            {
                PdfPage page = new PdfPage();
                page.Width = XUnit.FromMillimeter(90.3);
                page.Height = XUnit.FromMillimeter(29);
                doc.Pages.Add(page);

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont font = new XFont("Verdana", 11, XFontStyle.Bold);
                string itemText = item.Id.ToString() + " | " + item.Description;
                gfx.DrawString(itemText, font, XBrushes.Black, new XRect(0, 0, page.Width, 15), XStringFormats.TopLeft);

                XPen pen = new XPen(XColor.FromName("Black"), 1);
                gfx.DrawLine(pen, new XPoint(0, 60), new XPoint(400, 60));

                string itemPrice = item.Price.ToString() + " kr";
                gfx.DrawString(itemPrice, font, XBrushes.Black, new XRect(0, 65, page.Width, 15), XStringFormats.TopLeft);

                CreateBarcode(item.Id, gfx);
            }

            string filename = "etiketter-" + phone + ".pdf";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/") + filename;
            doc.Save(path);

            return filename;
        }


        private void CreateBarcode(int id, XGraphics gfx)
        {
            try
            {
                string file = System.Web.HttpContext.Current.Server.MapPath("~/tmp/") + "barcode_" + id + " " + DateTimeOffset.Now.ToUnixTimeSeconds() + ".png";
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                Image img = b.Encode(BarcodeLib.TYPE.CODE128, id.ToString(), Color.Black, Color.White, 290, 120);

                img.Save(file, System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();

                XImage ximg = XImage.FromFile(file);

                gfx.DrawImage(ximg, 150, 18, 100, 40);
                ximg.Dispose();

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