using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.Models
{
    public class ItemSearchResult
    {
        public bool Exists { get; set; }
        public string Description
        {
            get { return description; }
        }
        public List<ItemChangeLog> ChangeLogs
        {
            get { return changeLogs; }
        }

        private string description;
        private List<ItemChangeLog> changeLogs;
        
        public ItemSearchResult(Item item)
        {
            if (item != null)
            {
                this.Exists = true;
                SecondHandMarketContext ctx = new SecondHandMarketContext();
                changeLogs = ctx.ItemChangeLogs.Where(i => i.ItemId == item.Id).ToList();
                description = item.Id.ToString() + " | " + item.Description;
            }
            else
            {
                this.Exists = false;
            }
        }

    }
}