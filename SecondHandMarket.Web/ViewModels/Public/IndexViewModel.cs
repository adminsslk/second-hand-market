using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;
using SecondHandMarket.Web.Models;

namespace SecondHandMarket.Web.ViewModels.Public
{
    public class IndexViewModel : ViewModel
    {
        private HtmlString mapSection = new HtmlString("");
        public string SearchString { get; set; }
        public List<ItemSearchResult> SearchResults { get; set;}
        public HtmlString InfoSection { get; set; }
        public HtmlString OpeningHoursSection { get; set; }
        public HtmlString MapSection
        {
            get
            { return mapSection;  }
            set
            {
                string html = value.ToString();
                if (html.StartsWith("<p>"))
                {
                    html = html.Remove(0, 3);
                    html = html.Remove(html.Length - 5, 4);
                }

                mapSection = new HtmlString(html);            
            }
        }

        public bool IsSearch { get; set; }

        public IndexViewModel()
        {
            SearchResults = new List<ItemSearchResult>();
            InfoSection = new HtmlString("");
            OpeningHoursSection = new HtmlString("");
        }

    }
}