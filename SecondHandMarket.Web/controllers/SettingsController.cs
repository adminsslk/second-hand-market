using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecondHandMarket.Web.ViewModels.Settings;
using System.IO;
using System.Net;
using SecondHandMarket.Database;

namespace SecondHandMarket.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult PublicPage(int id)
        {
            PublicPageViewModel viewModel = new PublicPageViewModel();
            if (viewModel.GetLoggedOnUser().UserRole.Id == 2)
                return View(PublicPageViewModel.GetModel(id));
            else
                return RedirectToAction("checkout", "admin");
        }

        // GET: Settings
        public ActionResult Users()
        {
            UsersViewModel viewModel = new UsersViewModel();
            if (viewModel.GetLoggedOnUser().UserRole.Id == 2)
                return View(UsersViewModel.GetModel());
            else
                return RedirectToAction("checkout", "admin");
        }

        [HttpPost]
        public ActionResult Users(string phone, string firstName, string lastName, int pageSize, int pageIndex)
        {
            UsersViewModel viewModel = new UsersViewModel();
            if (viewModel.GetLoggedOnUser().UserRole.Id == 2)
                return View(UsersViewModel.GetModel(phone, firstName, lastName, pageSize, pageIndex));
            else
                return RedirectToAction("checkout", "admin");
        }
        // GET: Settings
        public ActionResult Content()
        {
            ContentViewModel viewModel = new ContentViewModel();
            if (viewModel.GetLoggedOnUser().UserRole.Id == 2)
                return View(ContentViewModel.GetModel());
            else
                return RedirectToAction("checkout", "admin");
        }

        // GET: Settings
        public ActionResult SystemSettings()
        {
            SystemSettingsViewModel viewModel = new SystemSettingsViewModel();
            if (viewModel.GetLoggedOnUser().UserRole.Id == 2)
                return View(SystemSettingsViewModel.GetModel());
            else
                return RedirectToAction("checkout", "admin");
        }

        public ActionResult _UserRow(int id)
        {
            return PartialView("Partials/_UserRow", UserViewModel.GetModel(id));
        }

        public ActionResult UpdateUserRole(int id, int roleId)
        {
            UserManager.UpdateUserRole(id, roleId);
            return PartialView("Partials/_UserRow", UserViewModel.GetModel(id));
        }

        public void UpdateMembership(int userId, bool isMember)
        {
            UserManager.UpdateMembership(userId, isMember);
        }

        public ActionResult _DeleteUserForm(int id)
        {
            return PartialView("Modals/_DeleteUserForm", UserViewModel.GetModel(id));
        }

        public ActionResult _RegisterUserForm()
        {
            return PartialView("Modals/_RegisterUserForm");
        }

        public ActionResult _EditUserForm(int id)
        {
            UserViewModel viewModel = UserViewModel.GetModel(id);
            return PartialView("Modals/_EditUserForm", viewModel);
        }

        public ActionResult _ChangePasswordForm(int id)
        {
            UserViewModel viewModel = UserViewModel.GetModel(id);
            return PartialView("Modals/_ChangePasswordForm", viewModel);
        }

        public void InsertUser(string firstName, string lastName, string email, string phone, string password, string isMember, string bank, string clearingNumber, string accountNumber, string roleId)
        {
            if (isMember == "on")
                isMember = "true";
            else
                isMember = "false";

            UserManager.InsertUser(firstName, lastName, email, phone, password, bool.Parse(isMember), bank, clearingNumber, accountNumber, int.Parse(roleId));
        }

        public void UpdateUser(int id, string firstName, string lastName, string email, string phone, string isMember, string bank, string clearingNumber, string accountNumber)
        {
            if (isMember == "on")
                isMember = "true";
            else
                isMember = "false";

            UserManager.UpdateUser(id, firstName, lastName, email, phone, bool.Parse(isMember), bank, clearingNumber, accountNumber);
        }

        public void ChangePassword(int id, string password)
        {
            UserManager.ChangePassword(id, password);
        }

        public void DeleteUser(int id)
        {
            UserManager.DeleteUser(id);
        }

        public JsonResult CheckIfEmailExists(string email)
        {
            bool exists = UserManager.CheckIfEmailExists(email);

            JsonResult j = this.Json(exists, JsonRequestBehavior.AllowGet);
            return j;
        }

        public JsonResult CheckIfPhoneExists(string phone)
        {
            bool exists = UserManager.CheckIfPhoneExists(phone);

            JsonResult j = this.Json(exists, JsonRequestBehavior.AllowGet);
            return j;
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/content/"), fileName);
                file.SaveAs(path);
            }

            return RedirectToAction("content");
        }

        [HttpPost]
        public ActionResult DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) == false)
            {
                string path = WebUtility.UrlDecode(fileName);
                path = Server.MapPath(path);
                System.IO.File.Delete(path);
            }

            return RedirectToAction("content");
        }

        [HttpPost]
        [ValidateInput(false)]
        public void SavePublicPageSection(int year, string sectionName, string html)
        {
            PublicPageViewModel.SavePublicPageSection(year, sectionName, html);
        }

        [HttpPost]
        public void UpdateActiveYear(Models.Year newYear)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Year year = ctx.Years.Find(newYear.Value);

            if (year == null)
                return;

            year.RevenueShare = newYear.RevenueShare;
            year.SalesCost = newYear.SalesCost;

            GlobalSetting gs = ctx.GlobalSettings.Find("ActiveYear");
            gs.Value = newYear.Value.ToString();

            ctx.SaveChanges();
        }

        public JsonResult GetYear(int key)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Year year = ctx.Years.Find(key);

            Models.Year jsonYear = null;
            if (year != null)
            {
                jsonYear = new Models.Year();
                jsonYear.Value = year.Value;
                jsonYear.RevenueShare = year.RevenueShare.Value;
                jsonYear.SalesCost = year.SalesCost.Value;
            }

            JsonResult j = this.Json(jsonYear, JsonRequestBehavior.AllowGet);
            return j;
        }
    }
}