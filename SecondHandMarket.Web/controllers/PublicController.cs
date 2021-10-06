using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecondHandMarket.Database;
using SecondHandMarket.Web.Models;
using SecondHandMarket.Web.ViewModels.Public;

namespace SecondHandMarket.Web.Controllers
{
    public class PublicController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            IndexViewModel viewModel = new IndexViewModel();

            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SearchString = "";
            viewModel.IsSearch = false;
            if(ctx.FrontPageSections.Where(s => s.Year == viewModel.ActiveYear && s.Section == "info").FirstOrDefault() != null)
                viewModel.InfoSection = new HtmlString(ctx.FrontPageSections.Where(s => s.Year == viewModel.ActiveYear && s.Section == "info").FirstOrDefault().Html);
            if (ctx.FrontPageSections.Where(s => s.Year == viewModel.ActiveYear && s.Section == "openinghours").FirstOrDefault() != null)
                viewModel.OpeningHoursSection = new HtmlString(ctx.FrontPageSections.Where(s => s.Year == viewModel.ActiveYear && s.Section == "openinghours").FirstOrDefault().Html);
            if (ctx.FrontPageSections.Where(s => s.Year == viewModel.ActiveYear && s.Section == "map").FirstOrDefault() != null)
                viewModel.MapSection = new HtmlString(ctx.FrontPageSections.Where(s => s.Year == viewModel.ActiveYear && s.Section == "map").FirstOrDefault().Html);

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(string search)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            IndexViewModel viewModel = new IndexViewModel();

            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SearchString = search;
            
            SecondHandMarket.Database.User user = ctx.Users.Where(u => u.Phone == search).FirstOrDefault();

            if (user != null)
            {
                foreach (Item item in user.Items.Where(i => i.Year == viewModel.ActiveYear))
                    viewModel.SearchResults.Add(new ItemSearchResult(item));
                if (viewModel.SearchResults.Count == 0)
                    viewModel.SearchResults.Add(new ItemSearchResult(null));
            }
            else
            {
                int itemId = 0;
                if (int.TryParse(search, out itemId))
                {
                    Item item = ctx.Items.Where(i => i.Year == viewModel.ActiveYear && i.Id == itemId).FirstOrDefault();
                    viewModel.SearchResults.Add(new ItemSearchResult(item));
                }
                else
                {
                    viewModel.SearchResults.Add(new ItemSearchResult(null));
                }
            }


            viewModel.IsSearch = true;
            if (String.IsNullOrEmpty(search))
                viewModel.IsSearch = false;

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult _ItemSearchResult(string id)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Item item = ctx.Items.Find(id);
                
            return PartialView(new ItemSearchResult(item));                
        }
    }
}