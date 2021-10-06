using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class SalesViewModel : ViewModel
    {
        public List<string> ItemDescriptions { get; set; }
        public List<Item> AddedItems { get; set; }
        public int? TotalPrice { get; set; }

        public SalesViewModel()
        {
            ItemDescriptions = new List<string>();
            AddedItems = new List<Item>();
            
        }

        public static SalesViewModel CreateViewModel()
        {
            SalesViewModel viewModel = new SalesViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);

            foreach (Item item in ctx.Items.Where(i => i.Year == viewModel.ActiveYear && i.ItemStatus.Id == 2))
                viewModel.ItemDescriptions.Add(item.Id.ToString() + " | " + item.Description);
            
            return viewModel;
        }

        public static SalesViewModel CreateViewModel(List<int> itemIds)
        {
            SalesViewModel viewModel = new SalesViewModel();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);

            foreach (Item item in ctx.Items.Where(i => i.Year == viewModel.ActiveYear && i.ItemStatus.Id == 2))
                if(itemIds.Contains(item.Id) == false)
                viewModel.ItemDescriptions.Add(item.Id.ToString() + " | " + item.Description);

            foreach (int id in itemIds)
            {
                Item item = ctx.Items.Where(i => i.Year == viewModel.ActiveYear && i.ItemStatus.Id == 2 && i.Id == id).FirstOrDefault();
                if (item != null)
                    viewModel.AddedItems.Add(item);
            }

            viewModel.TotalPrice = viewModel.AddedItems.Sum(i => i.Price);

            return viewModel;
        }
    }

    
}