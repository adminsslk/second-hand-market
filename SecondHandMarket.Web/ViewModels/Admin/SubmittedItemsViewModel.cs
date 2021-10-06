using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class SubmittedItemsViewModel : ViewModel
    {
        public User Salesman { get; set; }
        public List<Item> Items { get; set; }

        public static SubmittedItemsViewModel GetModel(string phone)
        {
            SubmittedItemsViewModel viewModel = new SubmittedItemsViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.Salesman = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.Items = ctx.Items.Where(i => i.SalemanId == viewModel.Salesman.Id && i.Year == viewModel.ActiveYear && i.StatusId == 2).ToList();
            
            return viewModel;
        }
    }
}