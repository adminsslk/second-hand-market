using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class SalesmanViewModel : ViewModel
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public int NumberOfRegiteredItems { get; set; }
        public int NumberOfSoldItems { get; set; }
        public int NumberOfPaidOutItems { get; set; }
        public int NumberOfReturnedItems { get; set; }
        public int AmoutToPayOut { get; set; }
        public string Member { get; set; }

        public static SalesmanViewModel GetModel(string phone, int year)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();

            if (user == null)
                return null;

            SalesmanViewModel viewModel = new SalesmanViewModel();
            viewModel.FullName = user.FullName;
            viewModel.Phone = user.Phone;
            viewModel.Bank = user.Bank;
            viewModel.BankAccount = user.ClearingNumber + " " + user.AccountNumber;
            viewModel.NumberOfRegiteredItems = user.Items.Where(i => i.Year == year && (i.StatusId == 2 | i.StatusId == 3 | i.StatusId == 4 | i.StatusId == 5)).Count();
            viewModel.NumberOfSoldItems = user.Items.Where(i => i.Year == year && (i.StatusId == 3 | i.StatusId == 5)).Count();
            viewModel.NumberOfPaidOutItems = user.Items.Where(i => i.Year == year && i.StatusId == 5).Count();
            viewModel.NumberOfReturnedItems = user.Items.Where(i => i.Year == year && i.StatusId == 4).Count();
            viewModel.AmoutToPayOut = user.Items.Where(i => i.Year == year && i.StatusId == 3).Sum(i => i.SellersShare) ?? 0;
            if (user.IsMember)
                viewModel.Member = "Ja";
            else
                viewModel.Member = "Nej";

            return viewModel;
        }

       
    }
}