using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Settings
{
    public class PublicPageViewModel : SettingsViewModel
    {
        public List<Year> Years { get; set; }
        public List<FrontPageSection> FrontPageSections { get; set; }
        public int SelectedYear { get; set; }

        public static PublicPageViewModel GetModel(int year)
        {
            return CreateViewModel(year);
        }

        private static PublicPageViewModel CreateViewModel(int year)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            PublicPageViewModel viewModel = new PublicPageViewModel();

            viewModel.Years = ctx.Years.OrderByDescending(y => y.Value).ToList();
            viewModel.FrontPageSections = ctx.FrontPageSections.Where(q => q.Year == year).ToList();
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SelectedYear = year;
            viewModel.SelectedTab = "settings";
            viewModel.SelectedSettingsTab = "publicpage";
            return viewModel;

        }

        public static void SavePublicPageSection(int year, string sectionName, string html)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            FrontPageSection section = ctx.FrontPageSections.Where(f => f.Section == sectionName && f.Year.Value == year).FirstOrDefault();

            if (section == null)
            {
                section = new FrontPageSection();
                section.Year = year;
                section.Section = sectionName;
                ctx.FrontPageSections.Add(section);
            }

            section.Html = html;

            ctx.SaveChanges();
        }
    }
}