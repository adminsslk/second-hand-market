using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class SoldItemsViewModel : ViewModel
    {
        public User Salesman { get; set; }
        public List<Item> Items { get; set; }
        public int? PayOutAmount { get; set; }

        public static SoldItemsViewModel GetModel(string phone)
        {
            SoldItemsViewModel viewModel = new SoldItemsViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.Salesman = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            if(viewModel.Salesman == null)
            {
                viewModel.Items = new List<Item>();
                viewModel.PayOutAmount = 0;
                return viewModel;
            }
            viewModel.Items = ctx.Items.Where(i => i.SalemanId == viewModel.Salesman.Id && i.Year == viewModel.ActiveYear && i.StatusId == 3).ToList();
            viewModel.PayOutAmount = viewModel.Items.Sum(q => q.SellersShare);
            return viewModel;
        }
    }
}