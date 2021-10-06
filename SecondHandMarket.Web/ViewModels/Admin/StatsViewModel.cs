using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class StatsViewModel : ViewModel
    {
        public List<SecondHandMarket.Web.Models.Stats> Stats { get; set; }
        public static StatsViewModel GetModel()
        {
            return CreateViewModel();
        }

        private static StatsViewModel CreateViewModel()
        {
            StatsViewModel viewModel = new StatsViewModel();
            viewModel.Stats = new List<SecondHandMarket.Web.Models.Stats>();
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SelectedTab = "stats";

            foreach (Year year in ctx.Years.Where(y => y.Value <= DateTime.Now.Year).Take(10).OrderByDescending(y => y.Value))
            {
                viewModel.Stats.Add(new SecondHandMarket.Web.Models.Stats(year.Value));
            }

            return viewModel;
        }
    }
}