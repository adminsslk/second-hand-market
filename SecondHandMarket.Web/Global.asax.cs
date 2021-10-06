using SecondHandMarket.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SecondHandMarket.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static User GetLoggedOnUser()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return ctx.Users.Where(u => u.Email == HttpContext.Current.User.Identity.Name).FirstOrDefault();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
