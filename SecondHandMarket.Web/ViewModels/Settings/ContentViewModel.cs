using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;
using System.IO;
using System.Configuration;

namespace SecondHandMarket.Web.ViewModels.Settings
{
    public class ContentViewModel : SettingsViewModel
    {
        public static ContentViewModel GetModel()
        {
            return CreateViewModel();
        }

        private static ContentViewModel CreateViewModel()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            ContentViewModel viewModel = new ContentViewModel();
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SelectedTab = "settings";
            viewModel.SelectedSettingsTab = "content";
            return viewModel;

        }

        public List<Uri> GetContentUris()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["MediaFolder"]));

            List<Uri> uris = new List<Uri>();
            string contentBaseUrl = (HttpContext.Current.Request.Url.GetComponents(
                    UriComponents.SchemeAndServer, UriFormat.Unescaped).TrimEnd('/')
                 + HttpContext.Current.Request.ApplicationPath) + "/" + ConfigurationManager.AppSettings["MediaFolder"] + "/";

            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                Uri contentUri = new Uri(contentBaseUrl + fileInfo.Name);
                uris.Add(contentUri);
            }

            return uris;
        }
    }
}