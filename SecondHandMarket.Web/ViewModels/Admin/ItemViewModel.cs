using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class ItemViewModel : ViewModel
    {
        public Item Item { get; set; }
        public List<ItemChangeLog> ChangeLogs { get; set; }

        public static ItemViewModel GetModel(int id)
        {
            return CreateViewModel(id);
        }

        private static ItemViewModel CreateViewModel(int id)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            ItemViewModel viewModel = new ItemViewModel();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.Item = ctx.Items.Find(id);
            viewModel.ChangeLogs = ctx.ItemChangeLogs.Where(i => i.ItemId == id && i.Year == viewModel.ActiveYear).ToList();

            return viewModel;

        }
    }
}