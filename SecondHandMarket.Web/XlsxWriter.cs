using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;

namespace SecondHandMarket.Web
{
    //Minimal xlsx med ett blad. Strängar skrivs som text så att Excel inte gör om
    //t.ex. clearingnummer till datum eller tar bort inledande nollor.
    public class XlsxWriter
    {
        private readonly List<object[]> rows = new List<object[]>();
        private readonly List<bool> boldRows = new List<bool>();
        private double[] columnWidths;

        public string SheetName { get; set; } = "Blad1";

        public void SetColumnWidths(params double[] widths)
        {
            columnWidths = widths;
        }

        //Värden av typen int/long/decimal/double blir tal, allt annat blir text
        public void AddRow(params object[] values)
        {
            rows.Add(values);
            boldRows.Add(false);
        }

        public void AddBoldRow(params object[] values)
        {
            rows.Add(values);
            boldRows.Add(true);
        }

        public byte[] ToArray()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    AddEntry(zip, "[Content_Types].xml",
                        "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                        "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                        "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                        "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                        "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                        "<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>" +
                        "</Types>");
                    AddEntry(zip, "_rels/.rels",
                        "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                        "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                        "</Relationships>");
                    AddEntry(zip, "xl/workbook.xml",
                        "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                        "<sheets><sheet name=\"" + Escape(SheetName) + "\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                        "</workbook>");
                    AddEntry(zip, "xl/_rels/workbook.xml.rels",
                        "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                        "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                        "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
                        "</Relationships>");
                    //Stil 0 = normal, stil 1 = fetstil
                    AddEntry(zip, "xl/styles.xml",
                        "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                        "<fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
                        "<fills count=\"2\"><fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill></fills>" +
                        "<borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders>" +
                        "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                        "<cellXfs count=\"2\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/><xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/></cellXfs>" +
                        "</styleSheet>");
                    AddEntry(zip, "xl/worksheets/sheet1.xml", SheetXml());
                }
                return stream.ToArray();
            }
        }

        private string SheetXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");

            //Lås rubrikraden
            xml.Append("<sheetViews><sheetView workbookViewId=\"0\"><pane ySplit=\"1\" topLeftCell=\"A2\" activePane=\"bottomLeft\" state=\"frozen\"/></sheetView></sheetViews>");

            if (columnWidths != null && columnWidths.Length > 0)
            {
                xml.Append("<cols>");
                for (int i = 0; i < columnWidths.Length; i++)
                    xml.Append("<col min=\"" + (i + 1) + "\" max=\"" + (i + 1) + "\" width=\"" + columnWidths[i].ToString(CultureInfo.InvariantCulture) + "\" customWidth=\"1\"/>");
                xml.Append("</cols>");
            }

            xml.Append("<sheetData>");
            for (int r = 0; r < rows.Count; r++)
            {
                string style = boldRows[r] ? " s=\"1\"" : "";
                xml.Append("<row r=\"" + (r + 1) + "\">");
                for (int c = 0; c < rows[r].Length; c++)
                {
                    object value = rows[r][c];
                    if (value == null)
                        continue;

                    string reference = ColumnName(c) + (r + 1);
                    if (value is int || value is long || value is decimal || value is double)
                        xml.Append("<c r=\"" + reference + "\"" + style + "><v>" + Convert.ToString(value, CultureInfo.InvariantCulture) + "</v></c>");
                    else
                        xml.Append("<c r=\"" + reference + "\" t=\"inlineStr\"" + style + "><is><t xml:space=\"preserve\">" + Escape(value.ToString()) + "</t></is></c>");
                }
                xml.Append("</row>");
            }
            xml.Append("</sheetData>");
            xml.Append("</worksheet>");
            return xml.ToString();
        }

        private static string ColumnName(int index)
        {
            string name = "";
            for (index++; index > 0; index = (index - 1) / 26)
                name = (char)('A' + (index - 1) % 26) + name;
            return name;
        }

        private static string Escape(string text)
        {
            //Tecken som inte är tillåtna i XML tas bort
            string cleaned = new string(text.Where(ch => ch == '\t' || ch == '\n' || ch == '\r' || ch >= ' ').ToArray());
            return SecurityElement.Escape(cleaned);
        }

        private static void AddEntry(ZipArchive zip, string name, string content)
        {
            ZipArchiveEntry entry = zip.CreateEntry(name);
            using (StreamWriter writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)))
            {
                writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                writer.Write(content);
            }
        }
    }
}
