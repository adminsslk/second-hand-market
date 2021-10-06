using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class ReceiptViewModel : ViewModel
    {
        public User Salesman { get; set; }
        public List<Item> Items { get; set; }

        public static ReceiptViewModel GetModel(string phone)
        {
            ReceiptViewModel viewModel = new ReceiptViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.Salesman = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.Items = ctx.Items.Where(i => i.SalemanId == viewModel.Salesman.Id && i.Year == viewModel.ActiveYear && i.StatusId != 1).ToList();

            return viewModel;
        }

        public void LogPrintOut()
        {
            //LOG PRINT OUT
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            ReceiptPrintOut printOut = new ReceiptPrintOut();
            printOut.SalesmanId = Salesman.Id;
            printOut.TimeStamp = DateTime.Now;
            printOut.Year = ActiveYear;
            printOut.UserName = GetLoggedOnUser().FullName;

            ctx.ReceiptPrintOuts.Add(printOut);
            ctx.SaveChanges();
        }
    }
}