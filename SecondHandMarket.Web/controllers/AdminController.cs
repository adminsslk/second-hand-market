using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecondHandMarket.Database;
using Excel;
using System.Text.RegularExpressions;
using SecondHandMarket.Web.ViewModels.Admin;
using Newtonsoft.Json;
using System.Net;

namespace SecondHandMarket.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public ActionResult Checkout()
        {
            return View(CheckoutViewModel.GetModel());
        }

        public ActionResult Items()
        {
            return View(ItemsViewModel.GetModel());
        }

        [HttpPost]
        public ActionResult Items(string phone, string id, int sid, int year, int pageSize, int pageIndex)
        {
            return View(ItemsViewModel.GetModel(phone, id, sid, year, pageSize, pageIndex));
        }

        [HttpGet]
        public ActionResult Salesmen()
        {
            return View(SalesmenViewModel.GetModel());
        }

        [HttpPost]
        public ActionResult Salesmen(SalesmenViewModel viewModel)
        {
            return View(SalesmenViewModel.GetModel(viewModel.FirstName, viewModel.LastName, viewModel.Phone, viewModel.Year, viewModel.PageSize, viewModel.PageIndex, viewModel.Member));
        }

        public ActionResult Stats(int? year)
        {
            StatsViewModel viewModel = new StatsViewModel();
            if (viewModel.GetLoggedOnUser().UserRole.Id == 2)
                return View(StatsViewModel.GetModel());
            else
                return RedirectToAction("items");
        }

        public string Label(string phone, int itemId)
        {
            LabelCreator creator = new LabelCreator();
            return "tmp/" + creator.CreateLabelPdf(phone, itemId);
        }

        public string Labels(string phone)
        {
            LabelCreator creator = new LabelCreator();
            return "tmp/" + creator.CreateLabelPdf(phone);
        }

        public ActionResult Receipt(string phone)
        {
            ReceiptViewModel viewModel = ReceiptViewModel.GetModel(phone);
            viewModel.LogPrintOut();
            return View(viewModel);
        }

        public ActionResult _SalesForm()
        {
            return PartialView("Modals/_SalesForm", SalesViewModel.CreateViewModel());
        }

        [HttpPost]
        public ActionResult _SalesFormBody(List<int> itemIds)
        {
            if (itemIds == null)
                return PartialView("Partials/_SalesFormBody", SalesViewModel.CreateViewModel());
            else
                return PartialView("Partials/_SalesFormBody", SalesViewModel.CreateViewModel(itemIds));
        }

        public ActionResult _RegisterSalesmanForm()
        {
            return PartialView("Modals/_RegisterSalesmanForm", RegisterSalesmanViewModel.CreateViewModel());
        }

        public ActionResult _SubmissionForm()
        {
            return PartialView("Modals/_SubmissionForm", RegisterSalesmanViewModel.CreateViewModel());
        }

        public ActionResult _PayOutForm(string phone)
        {
            return PartialView("Modals/_PayOutForm", PayOutViewModel.CreateViewModel(phone));
        }

        public ActionResult _ReturnForm()
        {
            return PartialView("Modals/_ReturnForm", ReturnViewModel.CreateViewModel());
        }

        public ActionResult _PrintOutForm(string phone, bool? showPrintReceiptButton = true)
        {
            PrintOutViewModel viewModel = PrintOutViewModel.GetModel(phone);
            viewModel.ShowPrintReceiptButton = showPrintReceiptButton.Value;
            return PartialView("Modals/_PrintOutForm", viewModel);
        }

        public ActionResult _RegisteredItems(string phone, bool? showSubmitted = false)
        {
            return PartialView("Partials/_RegisteredItems", RegisteredItemsViewModel.GetModel(phone, showSubmitted));
        }

        public ActionResult _SubmittedItems(string phone)
        {
            return PartialView("Partials/_SubmittedItems", SubmittedItemsViewModel.GetModel(phone));
        }

        public ActionResult _SoldItems(string phone)
        {
            return PartialView("Partials/_SoldItems", SoldItemsViewModel.GetModel(phone));
        }

        [HttpPost]
        public ActionResult _RegisteredItems(User salesman, List<Item> items)
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            viewModel.RegisterSalesman(salesman, items);
            return PartialView("Modals/_PrintOutForm",  PrintOutViewModel.GetModel(salesman.Phone));
        }

        public ActionResult _EditItemForm(int id)
        {
            return PartialView("Modals/_EditItemForm", ItemViewModel.GetModel(id));
        }

        public ActionResult _DeleteItemForm(int id)
        {
            return PartialView("Modals/_DeleteItemForm", ItemViewModel.GetModel(id));
        }

        public ActionResult _ItemRow(int id)
        {
            return PartialView("Partials/_ItemRow", ItemViewModel.GetModel(id));
        }

        public ActionResult _SalesmanRow(string phone, int year)
        {
            return PartialView("Partials/_SalesmanRow", SalesmanViewModel.GetModel(phone, year));
        }

        public void UpdateItem(int itemId, string description, int price, int sellerShare, int itemStatusId)
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            viewModel.UpdateItem(itemId, description, price, sellerShare, itemStatusId);
        }

        [HttpPost]
        public void UpdateNumberOfLabels(int itemId, int numberOfLabels)
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            viewModel.UpdateNumberOfLabels(itemId, numberOfLabels);
        }

        public void DeleteItem(int itemId)
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            viewModel.DeleteItem(itemId);         
        }

        public JsonResult CheckIfItemExists(int id)
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            bool exists = viewModel.CheckIfItemExists(id);
            JsonResult j = this.Json(exists, JsonRequestBehavior.AllowGet);
            return j;
        }

        public JsonResult GetRevenueShare()
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            decimal revenueShare = viewModel.GetRevenueShare();
            JsonResult j = this.Json(revenueShare, JsonRequestBehavior.AllowGet);
            return j;
        }        

        public JsonResult GetUser(string phone)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            User user = ctx.Users.Where(u => u.Phone == phone).FirstOrDefault();

            SecondHandMarket.Database.User jsonUser = new Database.User();
            if (user != null)
            {
                jsonUser.Phone = user.Phone;
                jsonUser.FirstName = user.FirstName;
                jsonUser.LastName = user.LastName;
                jsonUser.IsMember = user.IsMember;
            }

            JsonResult result = this.Json(jsonUser, JsonRequestBehavior.AllowGet);
            return result;
        }



        public ActionResult UpdateItemStatus(int id, int statusId)
        {
            ItemsViewModel viewModel = new ItemsViewModel();
            viewModel.UpdateItemStatus(id, statusId);
            return PartialView("Partials/_ItemRow", ItemViewModel.GetModel(id));
        }
        
        
    }
}