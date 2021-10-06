using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.ViewModels.Settings
{
    public class UserViewModel : ViewModel
    {
        public User User { get; set; }

        public static UserViewModel GetModel(int id)
        {
            return CreateViewModel(id);
        }

        public static UserViewModel GetModelByPhone(string phone)
        {
            return CreateViewModelByPhone(phone);
        }

        public static UserViewModel GetModelByEmail(string email)
        {
            return CreateViewModelByEmail(email);
        }

        private static UserViewModel CreateViewModel(int id)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            UserViewModel viewModel = new UserViewModel();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);

            if (viewModel.GetLoggedOnUser().UserRole.Id != 2)
                id = viewModel.GetLoggedOnUser().Id;

            viewModel.User = ctx.Users.Find(id);

            return viewModel;
        }

        private static UserViewModel CreateViewModelByPhone(string phone)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            UserViewModel viewModel = new UserViewModel();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.User = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();

            return viewModel;
        }

        private static UserViewModel CreateViewModelByEmail(string email)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            UserViewModel viewModel = new UserViewModel();
            viewModel.ActiveYear = int.Parse(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.User = ctx.Users.Where(u => u.Email == email).FirstOrDefault();

            return viewModel;
        }
    }
}