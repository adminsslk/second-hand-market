using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Admin
{
    public class SalesmenViewModel : ViewModel
    {
        public List<SalesmanViewModel> Users { get; set; }
        public int Year { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int SearchCount { get; set; }
        public int Member { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public static SalesmenViewModel GetModel()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return CreateViewModel("", "", "", Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value), 50, 0, 0);
        }

        public static SalesmenViewModel GetModel(string firstName, string lastName, string phone, int year, int pageSize, int pageIndex, int member)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return CreateViewModel(firstName, lastName, phone, year, pageSize, pageIndex, member);
        }

        private static SalesmenViewModel CreateViewModel(string firstName, string lastName, string phone, int year, int pageSize, int pageIndex, int member)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            SalesmenViewModel viewModel = new SalesmenViewModel();

            viewModel.Year = year;
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.Member = member;
            viewModel.FirstName = firstName;
            viewModel.LastName = lastName;
            viewModel.Phone = phone;
            viewModel.SelectedTab = "salesmen";

            List<User> users = ctx.Users.Where(u => u.Items.Where(i => i.Year == year && i.StatusId != 1).Count() > 0).ToList();

            //phone
            if (!String.IsNullOrEmpty(phone))
                users = users.Where(u => u.Phone.ToLower().StartsWith(phone.ToLower())).ToList();

            //firstName
            if (!String.IsNullOrEmpty(firstName))
                users = users.Where(u => u.FirstName.ToLower().StartsWith(firstName.ToLower())).ToList();

            //lastName
            if (!String.IsNullOrEmpty(lastName))
                users = users.Where(u => u.LastName.ToLower().StartsWith(lastName.ToLower())).ToList();

            viewModel.Users = new List<SalesmanViewModel>();
            foreach (User user in users.OrderBy(u => u.FirstName))
                viewModel.Users.Add(SalesmanViewModel.GetModel(user.Phone, year));

            viewModel.SearchCount = viewModel.Users.Count;
            viewModel.PageSize = pageSize;

            if (pageSize != 0)
                viewModel.PageCount = (int)Math.Ceiling((double)viewModel.SearchCount / (double)pageSize);
            else
                viewModel.PageCount = 1;

            if (viewModel.Users.Count > pageIndex)
            {
                viewModel.PageIndex = pageIndex;
                if (pageSize > 0)
                {
                    if (viewModel.Users.Count > (pageIndex + pageSize))
                        viewModel.Users = viewModel.Users.GetRange(pageIndex, pageSize);
                    else
                        viewModel.Users = viewModel.Users.GetRange(pageIndex, viewModel.Users.Count - pageIndex);
                }
            }

            return viewModel;
        }
    }
}