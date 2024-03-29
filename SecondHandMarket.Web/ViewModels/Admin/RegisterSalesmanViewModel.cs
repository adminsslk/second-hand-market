﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class RegisterSalesmanViewModel : ViewModel
    {
        public List<string> PhoneNumbers { get; set; }
        public List<ItemCategory> ItemCategories { get; set; }

        public static RegisterSalesmanViewModel CreateViewModel()
        {
            RegisterSalesmanViewModel viewModel = new RegisterSalesmanViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ItemCategories = ctx.ItemCategories.ToList();
            viewModel.PhoneNumbers = ctx.Users.Select(u => u.Phone).ToList<string>();
            return viewModel;
        }
    }
}