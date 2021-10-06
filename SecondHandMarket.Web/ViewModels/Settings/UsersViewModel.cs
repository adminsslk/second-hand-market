using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Settings
{
    public class UsersViewModel : SettingsViewModel
    {
        public List<User> Users { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int SearchCount { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static UsersViewModel GetModel()
        {
            return CreateViewModel("", "", "", 50, 0);
        }

        public static UsersViewModel GetModel(string phone, string firstName, string lastName, int pageSize, int pageIndex)
        {
            return CreateViewModel(phone, firstName, lastName, pageSize, pageIndex);
        }

        private static UsersViewModel CreateViewModel(string phone, string firstName, string lastName, int pageSize, int pageIndex)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            UsersViewModel viewModel = new UsersViewModel();
            
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.SelectedTab = "settings";
            viewModel.SelectedSettingsTab = "users";
            viewModel.Phone = phone;
            viewModel.FirstName = firstName;
            viewModel.LastName = lastName;

            IQueryable<User> users = ctx.Users;

            //phone
            if (phone != "")
                users = ctx.Users.Where(u => u.Phone.StartsWith(phone));

            //firstName
            if (firstName != "")
                users = users.Where(u => u.FirstName.StartsWith(firstName));

            //lastName
            if (lastName != "")
                users = users.Where(u => u.LastName.StartsWith(lastName));

            viewModel.Users = users.OrderBy(u => u.FirstName).ToList();
            viewModel.SearchCount = viewModel.Users.Count;
            viewModel.PageSize = pageSize;

            if (pageSize != 0)
                viewModel.PageCount = (int)Math.Ceiling((double)viewModel.SearchCount / (double)pageSize);
            else
                viewModel.PageCount = 1;

            if (viewModel.Users.Count > pageIndex)
            {
                viewModel.PageIndex = pageIndex;
                if (viewModel.Users.Count > (pageIndex + pageSize))
                    viewModel.Users = viewModel.Users.GetRange(pageIndex, pageSize);
                else
                    viewModel.Users = viewModel.Users.GetRange(pageIndex, viewModel.Users.Count - pageIndex);
            }

            return viewModel;

        }

    }
}