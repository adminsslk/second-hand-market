using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SecondHandMarket.Web
{
    public class ImportException : Exception
    {
        public ImportException(string message) : base(message) { }
    }

    public class ImportRow
    {
        public int RowNumber { get; set; }
        //Texten som hamnar på varan och etiketten: tagg + beskrivning + längd, förkortad så att den får plats
        public string Description { get; set; }
        //Tagg + beskrivning + längd som de stod i filen, för att kunna visa vad som förkortats
        public string FullDescription { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }

    //Läser varulistor från återförsäljare som CSV. Filen ska ha en rubrikrad med Beskrivning och Pris, och (frivilligt) Längd och Antal.
    public static class ItemImport
    {
        public const int LabelMaxLength = 26;
        public const int DescriptionMaxLength = 200;
        public const int MaxQuantity = 100;
        public const int MaxLabelsPerItem = 4;
        public const int TagMaxLength = 3;
        public const string DefaultTag = "*";

        private static readonly string[] DescriptionHeaders = { "beskrivning", "benämning", "vara" };
        private static readonly string[] LengthHeaders = { "längd", "langd" };
        private static readonly string[] PriceHeaders = { "pris" };
        private static readonly string[] QuantityHeaders = { "antal" };
        private const int HeaderSearchRows = 20;

        //Taggen sätts först i beskrivningen, t.ex. "*", för att visa att varan kommer från en återförsäljare
        public static List<ImportRow> Read(Stream stream, string fileName, string tag)
        {
            tag = tag ?? "";
            if (tag.Length > TagMaxLength)
                throw new ImportException("Taggen får vara högst " + TagMaxLength + " tecken.");

            if (Path.GetExtension(fileName ?? "").ToLowerInvariant() != ".csv")
                throw new ImportException("Filen måste vara en CSV-fil. Spara listan i Excel med Spara som och filformatet CSV.");

            string text = ReadText(stream);
            List<List<string>> records = ParseCsv(text, DetectDelimiter(text));

            //Hitta rubrikraden
            int headerRow = -1, descriptionColumn = -1, lengthColumn = -1, priceColumn = -1, quantityColumn = -1;
            for (int r = 0; r < Math.Min(records.Count, HeaderSearchRows) && headerRow < 0; r++)
            {
                int d = -1, l = -1, p = -1, q = -1;
                for (int c = 0; c < records[r].Count; c++)
                {
                    string header = Clean(records[r][c]).ToLowerInvariant();
                    if (d < 0 && DescriptionHeaders.Contains(header)) d = c;
                    else if (l < 0 && LengthHeaders.Contains(header)) l = c;
                    else if (p < 0 && PriceHeaders.Contains(header)) p = c;
                    else if (q < 0 && QuantityHeaders.Contains(header)) q = c;
                }
                if (d >= 0 && p >= 0)
                {
                    headerRow = r;
                    descriptionColumn = d;
                    lengthColumn = l;
                    priceColumn = p;
                    quantityColumn = q;
                }
            }

            if (headerRow < 0)
                throw new ImportException("Hittade ingen rubrikrad med kolumnerna Beskrivning och Pris i filen.");

            List<ImportRow> rows = new List<ImportRow>();
            for (int r = headerRow + 1; r < records.Count; r++)
            {
                List<string> record = records[r];
                string description = Cell(record, descriptionColumn);
                string length = NormalizeLength(Cell(record, lengthColumn));
                string price = NormalizeNumber(Cell(record, priceColumn));
                string quantity = NormalizeNumber(Cell(record, quantityColumn));

                if (description == "" && length == "" && price == "" && quantity == "")
                    continue;

                ImportRow row = new ImportRow();
                row.RowNumber = r + 1;
                row.FullDescription = tag + (length == "" ? description : (description + " " + length).Trim());
                row.Description = LabelText(tag, description, length);
                row.Price = price;
                row.Quantity = quantity == "" ? "1" : quantity;
                rows.Add(row);
            }

            if (rows.Count == 0)
                throw new ImportException("Filen innehåller inga varor under rubrikraden.");

            return rows;
        }

        //"<tagg>Beskrivning <längd>", där beskrivningen förkortas så att allt får plats på etiketten
        public static string LabelText(string tag, string description, string length)
        {
            if (description == "")
                return "";

            string suffix = length == "" ? "" : " " + length;
            int maxDescriptionLength = LabelMaxLength - tag.Length - suffix.Length;
            if (maxDescriptionLength < 1)
                return tag + description + suffix;

            return tag + Truncate(description, maxDescriptionLength) + suffix;
        }

        //Samma regler som i _ImportForm.js, men det är dessa som gäller när varorna sparas
        public static List<string> Validate(ImportRow row)
        {
            List<string> errors = new List<string>();
            string description = (row.Description ?? "").Trim();
            int price, quantity;

            if (description == "")
                errors.Add("Beskrivning saknas");
            else if (description.Length > DescriptionMaxLength)
                errors.Add("Beskrivningen är längre än " + DescriptionMaxLength + " tecken");

            if (!TryParsePositiveInt(row.Price, out price))
                errors.Add("Priset måste vara ett heltal större än 0");

            if (!TryParsePositiveInt(row.Quantity, out quantity) || quantity > MaxQuantity)
                errors.Add("Antal måste vara ett heltal mellan 1 och " + MaxQuantity);

            return errors;
        }

        public static bool TryParsePositiveInt(string text, out int value)
        {
            value = 0;
            return text != null && Regex.IsMatch(text.Trim(), @"^\d{1,9}$") && int.TryParse(text.Trim(), out value) && value > 0;
        }

        //Excel sparar CSV som Windows-1252, eller UTF-8 med BOM om man väljer "CSV UTF-8"
        private static string ReadText(Stream stream)
        {
            byte[] bytes;
            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                bytes = memory.ToArray();
            }

            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);

            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(1252).GetString(bytes);
            }
        }

        //Svenska Excel använder semikolon, men komma och tabb förekommer också. Avgör utifrån rubrikraden.
        private static char DetectDelimiter(string text)
        {
            string[] lines = text.Split('\n');
            string line = lines.FirstOrDefault(l => l.ToLowerInvariant().Contains("beskrivning")) ?? lines[0];

            char[] candidates = { ';', '\t', ',' };
            return candidates.OrderByDescending(c => line.Count(x => x == c)).First();
        }

        private static List<List<string>> ParseCsv(string text, char delimiter)
        {
            List<List<string>> records = new List<List<string>>();
            List<string> record = new List<string>();
            StringBuilder field = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (inQuotes)
                {
                    if (c == '"' && i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else if (c == '"')
                        inQuotes = false;
                    else
                        field.Append(c);
                }
                else if (c == '"')
                    inQuotes = true;
                else if (c == delimiter)
                {
                    record.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\n')
                {
                    record.Add(field.ToString());
                    field.Clear();
                    records.Add(record);
                    record = new List<string>();
                }
                else if (c != '\r')
                    field.Append(c);
            }

            if (field.Length > 0 || record.Count > 0)
            {
                record.Add(field.ToString());
                records.Add(record);
            }

            return records;
        }

        private static string Cell(List<string> record, int column)
        {
            return column >= 0 && column < record.Count ? Clean(record[column]) : "";
        }

        private static string Clean(string text)
        {
            return Regex.Replace(text ?? "", @"[\s ]+", " ").Trim();
        }

        private static string Truncate(string text, int maxLength)
        {
            return text.Length <= maxLength ? text : text.Substring(0, maxLength).TrimEnd();
        }

        //"165,0" -> "165". Annat, t.ex. "165 cm", lämnas som det är.
        private static string NormalizeLength(string text)
        {
            Match m = Regex.Match(text, @"^(\d+)[.,]0+$");
            return m.Success ? m.Groups[1].Value : text;
        }

        //"1 200 kr" -> "1200", "150,00" -> "150", "3 st" -> "3". Annat lämnas orört så att det syns som fel.
        private static string NormalizeNumber(string text)
        {
            string s = Regex.Replace(text, @"[\s ]", "");
            s = Regex.Replace(s, @"(kr|st|:-)$", "", RegexOptions.IgnoreCase);
            Match m = Regex.Match(s, @"^(\d+)(?:[.,]0+)?$");
            return m.Success ? m.Groups[1].Value : text;
        }
    }
}
