using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class ImportResult
    {
        public string Phone { get; set; }
        public int ItemCount { get; set; }
        //Kvittensen ligger i ~/tmp/
        public string ReceiptFile { get; set; }
    }

    public class ImportViewModel : ViewModel
    {
        public List<User> Wholesalers { get; set; }

        //Förhandsgranskning
        public User Salesman { get; set; }
        public List<ImportRow> Rows { get; set; }
        public int ExistingItemCount { get; set; }
        public string ErrorMessage { get; set; }

        public static ImportViewModel CreateViewModel()
        {
            ImportViewModel viewModel = new ImportViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.Wholesalers = ctx.Users
                .Where(u => (u.RoleId == SalesmanRoles.Wholesale || u.RoleId == SalesmanRoles.ClubWholesale) && u.Phone != null && u.Phone != "")
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .ToList();
            return viewModel;
        }

        public static ImportViewModel CreatePreview(int salesmanId, string tag, Stream file, string fileName)
        {
            ImportViewModel viewModel = new ImportViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.Salesman = GetWholesaler(ctx, salesmanId);

            if (viewModel.Salesman == null)
            {
                viewModel.ErrorMessage = "Välj en återförsäljare.";
                return viewModel;
            }

            if (file == null)
            {
                viewModel.ErrorMessage = "Välj en CSV-fil.";
                return viewModel;
            }

            try
            {
                viewModel.Rows = ItemImport.Read(file, fileName, tag);
            }
            catch (ImportException e)
            {
                viewModel.ErrorMessage = e.Message;
                return viewModel;
            }

            int salesmanIdValue = viewModel.Salesman.Id;
            int year = viewModel.ActiveYear;
            viewModel.ExistingItemCount = ctx.Items.Count(i => i.SalemanId == salesmanIdValue && i.Year == year);
            return viewModel;
        }

        //Sparar raderna som nya varor och skriver en kvittens. Kastar ImportException om något är fel.
        public ImportResult ImportItems(int salesmanId, List<ImportRow> rows)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User wholesaler = GetWholesaler(ctx, salesmanId);
            if (wholesaler == null)
                throw new ImportException("Återförsäljaren finns inte, eller har inget mobilnummer.");

            if (rows == null || rows.Count == 0)
                throw new ImportException("Det finns inga varor att importera.");

            List<Item> items = new List<Item>();
            List<KeyValuePair<ImportRow, Item>> imported = new List<KeyValuePair<ImportRow, Item>>();
            foreach (ImportRow row in rows)
            {
                List<string> errors = ItemImport.Validate(row);
                if (errors.Count > 0)
                    throw new ImportException("Rad " + row.RowNumber + ": " + string.Join(", ", errors));

                int price, quantity;
                ItemImport.TryParsePositiveInt(row.Price, out price);
                ItemImport.TryParsePositiveInt(row.Quantity, out quantity);

                for (int i = 0; i < quantity; i++)
                {
                    Item item = new Item();
                    item.Description = row.Description.Trim().ToUpper();
                    item.Price = price;
                    items.Add(item);
                    imported.Add(new KeyValuePair<ImportRow, Item>(row, item));
                }
            }

            //RegisterSalesman letar upp säljaren på mobilnummer och skriver över namn och medlemskap, så skicka med de befintliga värdena
            User salesman = new User();
            salesman.Phone = wholesaler.Phone;
            salesman.FirstName = wholesaler.FirstName ?? "";
            salesman.LastName = wholesaler.LastName ?? "";
            salesman.IsMember = wholesaler.IsMember;

            ItemsViewModel itemsViewModel = new ItemsViewModel();
            itemsViewModel.RegisterSalesman(salesman, items);

            //Varorna har nu fått sina varunummer
            ImportResult result = new ImportResult();
            result.Phone = wholesaler.Phone;
            result.ItemCount = items.Count;
            result.ReceiptFile = WriteReceipt(wholesaler, imported, itemsViewModel.GetLoggedOnUser());
            return result;
        }

        //CSV med semikolon och UTF-8 med BOM, så att svenska Excel öppnar den rätt
        private static string WriteReceipt(User wholesaler, List<KeyValuePair<ImportRow, Item>> imported, User importedBy)
        {
            DateTime now = DateTime.Now;
            StringBuilder csv = new StringBuilder();
            csv.AppendLine(CsvLine("Kvittens import"));
            csv.AppendLine(CsvLine("Återförsäljare", wholesaler.FullName + " (" + wholesaler.Phone + ")"));
            csv.AppendLine(CsvLine("Importerad", now.ToString("yyyy-MM-dd HH:mm") + (importedBy != null ? " av " + importedBy.FullName : "")));
            csv.AppendLine(CsvLine("Antal varor", imported.Count.ToString()));
            csv.AppendLine(CsvLine("Totalt pris", imported.Sum(i => i.Value.Price ?? 0) + " kr"));
            csv.AppendLine(CsvLine("Säljarens andel", imported.Sum(i => i.Value.SellersShare ?? 0) + " kr"));
            csv.AppendLine();
            csv.AppendLine(CsvLine("Rad i filen", "Varunummer", "Beskrivning", "Pris", "Säljarens andel"));
            foreach (KeyValuePair<ImportRow, Item> pair in imported)
            {
                Item item = pair.Value;
                csv.AppendLine(CsvLine(pair.Key.RowNumber.ToString(), item.Id.ToString(), item.Description, item.Price.ToString(), item.SellersShare.ToString()));
            }

            string filename = "kvittens-import-" + wholesaler.Phone + "-" + now.ToString("yyyyMMdd-HHmmss") + ".csv";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/") + filename;
            File.WriteAllText(path, csv.ToString(), new UTF8Encoding(true));
            return filename;
        }

        private static string CsvLine(params string[] fields)
        {
            return string.Join(";", fields.Select(f =>
            {
                f = f ?? "";
                return f.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0 ? "\"" + f.Replace("\"", "\"\"") + "\"" : f;
            }));
        }

        private static User GetWholesaler(SecondHandMarketContext ctx, int salesmanId)
        {
            User user = ctx.Users.Find(salesmanId);
            if (!SalesmanRoles.IsWholesale(user) || string.IsNullOrEmpty(user.Phone))
                return null;
            return user;
        }
    }
}
