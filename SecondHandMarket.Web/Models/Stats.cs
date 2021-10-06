using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;


namespace SecondHandMarket.Web.Models
{
    public class Stats : System.Collections.Hashtable
    {
        public int Year { get; set; }
        public Stats()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            int year = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            Init(year);
        }

        public Stats(int year)
        {
            Init(year);
        }

        private void Init(int year)
        {
            Year = year;
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            List<Item> items = ctx.Items.Where(i => i.Year == year).ToList();

            //MONEY
            int? stockValue = items.Sum(i => i.Price);
            this[2] = stockValue.Value.ToString("#,##0") + " kr";
            int? soldAmount = items.Where(i => i.StatusId == 3).Sum(i => i.Price) + items.Where(i => i.StatusId == 5).Sum(i => i.Price);
            this[3] = soldAmount.Value.ToString("#,##0") + " kr";
            int? repayedAmount = items.Where(i => i.StatusId == 5).Sum(i => i.SellersShare);
            this[5] = repayedAmount.Value.ToString("#,##0") + " kr";
            int? cash = Convert.ToInt32(soldAmount) - Convert.ToInt32(repayedAmount);
            this[6] = cash.Value.ToString("#,##0") + " kr";
            int? notRepayedAmount = items.Where(i => i.StatusId == 3).Sum(i => i.SellersShare);
            int? notRepayedAmount2 = items.Where(i => i.StatusId == 3 && i.Salesman.RoleId != 5).Sum(i => i.SellersShare);

            //COUNT
            int? stockCount = items.Count();
            int? soldCount = items.Where(i => i.StatusId == 3 || i.StatusId == 5).Count();
            int? repayedCount = items.Where(i => i.StatusId == 5).Count();
            int? adjustedStockCount = items.Where(i => i.Salesman.RoleId != 5).Count();
            int? notRepayedCount = soldCount - repayedCount;
            int? notRepayedCount2 = items.Where(i => i.StatusId == 3 && i.Salesman.RoleId != 5).Count();
            this[7] = stockCount.Value.ToString("#,##0");
            this[8] = soldCount.Value.ToString("#,##0");
            this[9] = repayedCount.Value.ToString("#,##0");

            //PROFIT
            int? salesCost = Convert.ToInt32(ctx.Years.Find(year).SalesCost);

            int? salesCostRevenue = adjustedStockCount * salesCost;
            int? salesProvision = soldAmount - (items.Where(i => i.StatusId == 3).Sum(i => i.SellersShare) + items.Where(i => i.StatusId == 5).Sum(i => i.SellersShare));
            int? profit = salesCostRevenue + salesProvision;
            this[10] = salesCost.Value.ToString("#,##0") + " kr";
            this[11] = salesCostRevenue.Value.ToString("#,##0") + " kr";
            this[12] = salesProvision.Value.ToString("#,##0") + " kr";
            this[13] = profit.Value.ToString("#,##0") + " kr";

            this[14] = adjustedStockCount.Value.ToString("#,##0");
            this[15] = notRepayedCount.Value.ToString("#,##0");
            this[16] = notRepayedAmount.Value.ToString("#,##0") + " kr";

            this[17] = notRepayedCount2.Value.ToString("#,##0");
            this[18] = notRepayedAmount2.Value.ToString("#,##0") + " kr";
        }
    }
}