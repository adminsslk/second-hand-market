using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;
using System.IO;
using System.Configuration;

namespace SecondHandMarket.Web.ViewModels.Settings
{
    public class SystemSettingsViewModel : SettingsViewModel
    {
        public static SystemSettingsViewModel GetModel()
        {
            return CreateViewModel();
        }

        private static SystemSettingsViewModel CreateViewModel()
        {
            AddNextYear();

            SecondHandMarketContext ctx = new SecondHandMarketContext();
            SystemSettingsViewModel viewModel = new SystemSettingsViewModel();
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SelectedTab = "settings";
            viewModel.SelectedSettingsTab = "systemsettings";
            return viewModel;

        }

        private static void AddNextYear()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            int nextYear = DateTime.Now.Year + 1;
            Year maxYear = ctx.Years.OrderByDescending(y => y.Value).FirstOrDefault();

            if (maxYear != null && maxYear.Value < nextYear)
            {
                Year y = new Year();
                y.Value = nextYear;

                var html = "";
                if (maxYear.FrontPageSections.FirstOrDefault(x => x.Section == "info") != null)
                    html = maxYear.FrontPageSections.Where(x => x.Section == "info").FirstOrDefault().Html;
                FrontPageSection f = new FrontPageSection();
                f.Year = y.Value;
                f.Section = "info";
                f.Html = html ;
                y.FrontPageSections.Add(f);

                html = "";
                if (maxYear.FrontPageSections.FirstOrDefault(x => x.Section == "openinghours") != null)
                    html = maxYear.FrontPageSections.Where(x => x.Section == "openinghours").FirstOrDefault().Html;
                f = new FrontPageSection();
                f.Year = y.Value;
                f.Section = "openinghours";
                f.Html = html;
                y.FrontPageSections.Add(f);

                html = "";
                if (maxYear.FrontPageSections.FirstOrDefault(x => x.Section == "map") != null)
                    html = maxYear.FrontPageSections.Where(x => x.Section == "map").FirstOrDefault().Html;
                f = new FrontPageSection();
                f.Year = y.Value;
                f.Section = "map";
                f.Html = html;
                y.FrontPageSections.Add(f);

                if (maxYear.RevenueShare.HasValue)
                    y.RevenueShare = maxYear.RevenueShare;
                else
                    y.RevenueShare = 0.15;
                if (maxYear.SalesCost.HasValue)
                    y.SalesCost = maxYear.SalesCost;
                else
                    y.SalesCost = 30;

                ctx.Years.Add(y);

                ctx.SaveChanges();
            }
        }
    }
}