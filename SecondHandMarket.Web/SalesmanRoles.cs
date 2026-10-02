using System;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web
{
    public static class SalesmanRoles
    {
        public const int Wholesale = 5;
        //Klubbens egen försäljning: som en återförsäljare, men allt går till klubben och ingen inlämningsavgift tas ut
        public const int ClubWholesale = 6;
        public const string ClubWholesaleName = "wholesale - SSLK";

        //Återförsäljare av båda slagen, t.ex. de som kan importera varulistor
        public static bool IsWholesale(User user)
        {
            return user != null && (user.RoleId == Wholesale || user.RoleId == ClubWholesale);
        }

        public static bool PaysRegistrationFee(User user)
        {
            return user == null || user.RoleId != ClubWholesale;
        }

        //Rollen läggs in första gången den används, så att den inte behöver skapas för hand i databasen
        public static void EnsureRoleExists(SecondHandMarketContext ctx, int roleId)
        {
            if (roleId != ClubWholesale || ctx.UserRoles.Find(roleId) != null)
                return;

            UserRole role = new UserRole();
            role.Id = ClubWholesale;
            role.Name = ClubWholesaleName;
            ctx.UserRoles.Add(role);
        }
    }
}
