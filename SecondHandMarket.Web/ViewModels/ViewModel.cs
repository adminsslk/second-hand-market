using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels
{
    public abstract class ViewModel
    {
        public string SelectedTab { get; set; }
        public int ActiveYear { get; set; }

        public User GetLoggedOnUser()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return ctx.Users.Where(u => u.Email == HttpContext.Current.User.Identity.Name).FirstOrDefault();
        }

        protected static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        public decimal GetRevenueShare()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            int year = DateTime.Now.Year;
            string setting = ctx.GlobalSettings.Find("ActiveYear").Value;
            if (setting != null)
                year = Convert.ToInt32(setting);

            return Convert.ToDecimal(ctx.Years.Find(year).RevenueShare);
        }

        public decimal GetSalesCost()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return Convert.ToDecimal(ctx.Years.Find(ActiveYear).SalesCost);
        }

        public List<Year> GetYears()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return ctx.Years.OrderByDescending(y => y.Value).ToList();
        }
    }
}