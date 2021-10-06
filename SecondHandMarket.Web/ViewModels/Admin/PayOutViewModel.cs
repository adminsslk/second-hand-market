using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class PayOutViewModel : ViewModel
    {
        public List<string> PhoneNumbers { get; set; }
        public List<ItemCategory> ItemCategories { get; set; }
        public string Phone { get; set; }

        public static PayOutViewModel CreateViewModel(string phone)
        {
            PayOutViewModel viewModel = new PayOutViewModel();
            viewModel.Phone = phone;
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ItemCategories = ctx.ItemCategories.ToList();
            viewModel.PhoneNumbers = ctx.Users.Select(u => u.Phone).ToList<string>();
            return viewModel;
        }
    }
}