using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account/Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Message = "";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user != null && user.Password == password && (user.RoleId == 2 || user.RoleId == 3))
            {
                FormsAuthentication.SetAuthCookie(email, true);
                return RedirectToAction("checkout", "admin");
            }

            ViewBag.Message = "Inloggningen misslyckades";
            return View();
        }

        [HttpGet]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("index", "public");
        }

    }
}