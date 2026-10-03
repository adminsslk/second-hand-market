using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
	public class PrintOutViewModel : ViewModel
	{
        public User Salesman { get; set; }
        public List<Item> Items { get; set; }
        public string RegistrationFee { get; set; }

        public bool ShowPrintReceiptButton { get; set; }

        //Sätts efter en import
        public ImportResult ImportResult { get; set; }

        public static PrintOutViewModel GetModel(string phone)
        {
            PrintOutViewModel viewModel = new PrintOutViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.Salesman = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.Items = ctx.Items.Where(i => i.SalemanId == viewModel.Salesman.Id && i.Year == viewModel.ActiveYear).ToList();
            int salesCost = Convert.ToInt32(ctx.Years.Find(viewModel.ActiveYear).SalesCost);
            if (!SalesmanRoles.PaysRegistrationFee(viewModel.Salesman))
                salesCost = 0;
            viewModel.RegistrationFee = (salesCost * viewModel.Items.Count).ToString() + " kr";

            return viewModel;
        }

    }
}