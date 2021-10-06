using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class RegisteredItemsViewModel : ViewModel
    {
        public User Salesman { get; set; }
        public List<Item> Items { get; set; }
        public string RegistrationFee { get; set; }
        public bool ShowSubmitted { get; set; }

        public static RegisteredItemsViewModel GetModel(string phone, bool? showSubmitted = false)
        {
            RegisteredItemsViewModel viewModel = new RegisteredItemsViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ShowSubmitted = showSubmitted.Value;
            viewModel.Salesman = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);

            if (viewModel.Salesman == null)
            {
                viewModel.Items = new List<Item>();
                return viewModel;
            }
                

            if(showSubmitted == true)
                viewModel.Items = ctx.Items.Where(i => i.SalemanId == viewModel.Salesman.Id && i.Year == viewModel.ActiveYear && (i.StatusId == 1 | i.StatusId == 2) ).ToList();
            else
                viewModel.Items = ctx.Items.Where(i => i.SalemanId == viewModel.Salesman.Id && i.Year == viewModel.ActiveYear && i.StatusId == 1).ToList();
            int salesCost = Convert.ToInt32(ctx.Years.Find(viewModel.ActiveYear).SalesCost);
            viewModel.RegistrationFee = (salesCost * viewModel.Items.Count).ToString() + " kr";

            return viewModel;
        }
    }
}