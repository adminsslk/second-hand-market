using System;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web
{
    public static class ItemExtensions
    {
        //Säljarens andel i procent av priset, t.ex. "(85 %)". Tom sträng om det inte går att räkna ut.
        public static string SellersSharePercent(this Item item)
        {
            if (!item.Price.HasValue || item.Price.Value == 0 || !item.SellersShare.HasValue)
                return "";

            double percent = 100.0 * item.SellersShare.Value / item.Price.Value;
            return "(" + Math.Round(percent).ToString() + " %)";
        }
    }
}
