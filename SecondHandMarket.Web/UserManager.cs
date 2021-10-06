using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web
{
    public class UserManager
    {
        public static void InsertUser(string firstName, string lastName, string email, string phone, string password, bool isMember, string bank, string clearingNumber, string accountNumber, int roleId)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = new Core.User();
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.Phone = phone;
            user.Password = password;
            user.IsMember = isMember;
            user.Bank = bank;
            user.ClearingNumber = clearingNumber;
            user.AccountNumber = accountNumber;
            user.RoleId = roleId;
            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        public static void UpdateUser(int id, string firstName, string lastName, string email, string phone, bool isMember, string bank, string clearingNumber, string accountNumber)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Find(id);
            if (user == null)
                return;

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.Phone = phone;
            user.IsMember = isMember;
            user.Bank = bank;
            user.ClearingNumber = clearingNumber;
            user.AccountNumber = accountNumber;

            ctx.SaveChanges();
        }
        public static void ChangePassword(int id, string password)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Find(id);

            if (user == null)
                return;

            if (MvcApplication.GetLoggedOnUser().Id == id || MvcApplication.GetLoggedOnUser().UserRole.Id == 2)
            {
                user.Password = password;
                ctx.SaveChanges();
            }
        }

        public static void DeleteUser(int id)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Find(id);
            if (user != null)
            {
                if (user.Items.Count > 0)
                {
                    throw new Exception("Det finns registerade varor på användaren. Du måste ta bort dem innan du kan ta bort användaren.");
                }
                ctx.Users.Remove(user);
            }

            ctx.SaveChanges();
        }

        public static bool CheckIfEmailExists(string email)
        {
            bool exists = false;
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            if (ctx.Users.Where(u => u.Email == email).FirstOrDefault() != null)
                exists = true;

            return exists;
        }

        public static bool CheckIfPhoneExists(string phone)
        {
            bool exists = false;
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            if (ctx.Users.Where(u => u.Phone == phone).FirstOrDefault() != null)
                exists = true;

            return exists;
        }

        public static void UpdateUserRole(int id, int roleId)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Find(id);
            if (user != null)
            {
                user.RoleId = roleId;
                ctx.SaveChanges();
            }
        }

        public static void UpdateMembership(int id, bool isMember)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Find(id);
            if (user != null)
            {
                user.IsMember = isMember;
                ctx.SaveChanges();
            }
        }
    }
}