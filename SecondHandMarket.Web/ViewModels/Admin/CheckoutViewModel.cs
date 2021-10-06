using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class CheckoutViewModel : ViewModel
    {
        public static CheckoutViewModel GetModel()
        {
            return CreateViewModel();
        }

        private static CheckoutViewModel CreateViewModel()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            CheckoutViewModel viewModel = new CheckoutViewModel();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SelectedTab = "checkout";
            return viewModel;
        }
    }
}