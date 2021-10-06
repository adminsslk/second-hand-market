using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecondHandMarket.Core;

namespace SecondHandMarket.Web.ViewModels.Admin
{
	public class ItemsViewModel : ViewModel
	{
        public List<Item> Items { get; set; }
        public List<ItemStatus> ItemStatusList { get; set; }
        public string Phone { get; set; }
        public string Id { get; set; }
        public int Sid { get; set; }
        public int Year { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int SearchCount { get; set; }
        public ModalForm ModalFormToShow { get; set; }
        public string ModalFormPhone { get; set; }

        public static ItemsViewModel GetModel()
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            return CreateViewModel("", "", 0, Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value), 20, 0);
        }

        public static ItemsViewModel GetModel(string phone, string id, int sid, int year, int pageSize, int pageIndex)
        {
            return CreateViewModel(phone, id, sid, year, pageSize, pageIndex);
        }

        private static ItemsViewModel CreateViewModel(string phone, string id, int sid, int year, int pageSize, int pageIndex)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            ItemsViewModel viewModel = new ItemsViewModel();

            viewModel.Phone = phone;
            viewModel.Id = id;
            viewModel.Sid = sid;
            viewModel.Year = year;
            viewModel.ActiveYear = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);
            viewModel.ItemStatusList = ctx.ItemStatus.ToList();
            viewModel.SelectedTab = "items";

            IQueryable<Item> items = ctx.Items;

            //year
            if (year != 0)
                items = ctx.Items.Where(i => i.Year.Value == year);

            //phone
            if (phone != "")
                items = items.Where(i => i.Salesman.Phone.StartsWith(phone));

            //id
            if (id != "")
                items = items.Where(i => i.Id.ToString().StartsWith(id));

            //sid
            if(sid != 0)
                items = items.Where(i => i.StatusId == sid);
                        
            viewModel.Items = items.ToList();
            viewModel.SearchCount = viewModel.Items.Count;
            viewModel.PageSize = pageSize;

            if (pageSize != 0)
                viewModel.PageCount = (int)Math.Ceiling((double)viewModel.SearchCount / (double)pageSize);
            else
                viewModel.PageCount = 1;

            if (viewModel.Items.Count > pageIndex)
            {
                viewModel.PageIndex = pageIndex;
                if(pageSize > 0)
                {
                    if (viewModel.Items.Count > (pageIndex + pageSize))
                        viewModel.Items = viewModel.Items.GetRange(pageIndex, pageSize);
                    else
                        viewModel.Items = viewModel.Items.GetRange(pageIndex, viewModel.Items.Count - pageIndex);
                }                
            }
            
            return viewModel;
        }
        public void RegisterSalesman(User salesman, List<Item> items)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            decimal revenueShare = GetRevenueShare();
            int year = Convert.ToInt32(ctx.GlobalSettings.Find("ActiveYear").Value);

            salesman.Phone = salesman.Phone.Replace(" ", "");
            salesman.Phone = salesman.Phone.Replace("-", "");
            salesman.Phone = salesman.Phone.Replace("+", "");

            if (salesman.FirstName.Length > 1)
                salesman.FirstName = salesman.FirstName.Substring(0, 1).ToUpper() + salesman.FirstName.Substring(1);
            else
                salesman.FirstName = salesman.FirstName.ToUpper();

            if (salesman.LastName.Length > 1)
                salesman.LastName = salesman.LastName.Substring(0, 1).ToUpper() + salesman.LastName.Substring(1);
            else
                salesman.LastName = salesman.LastName.ToUpper();
                      

            //UPADTE OR INSERT SALESMAN
            if (ctx.Users.Where(u => u.Phone == salesman.Phone).FirstOrDefault() == null)
            {
                salesman.RoleId = 4;
                ctx.Users.Add(salesman);
            }
            else
            {
                string firstName = salesman.FirstName;
                string lastName = salesman.LastName;
                bool isMember = salesman.IsMember;
                salesman = ctx.Users.Where(u => u.Phone == salesman.Phone).FirstOrDefault();
                salesman.FirstName = firstName;
                salesman.LastName = lastName;
                salesman.IsMember = isMember;
            }

            ctx.SaveChanges();

            //INSERT ITEMS
            foreach (Item item in items) 
            {
                item.StatusId = 1;
                item.SalemanId = salesman.Id;
                item.Year = year;
                item.SellersShare = Convert.ToInt32(Math.Floor((1 - revenueShare) * item.Price.Value));
                item.NumberOfLabels = 1;
                ctx.Items.Add(item);

                if (item.Description.ToUpper().Contains("SKIDOR"))
                    item.NumberOfLabels = 2;
                if (item.Description.ToUpper().Contains("PJÄXOR"))
                    item.NumberOfLabels = 2;
                if (item.Description.ToUpper().Contains("SKOR"))
                    item.NumberOfLabels = 2;
                if (item.Description.ToUpper().Contains("TWINTIPS"))
                    item.NumberOfLabels = 2;

                ctx.SaveChanges();

                //LOG CHANGES
                ItemChangeLog statusLog = new ItemChangeLog();
                statusLog.TimeStamp = DateTime.Now;
                statusLog.ItemId = item.Id;
                statusLog.Log = ctx.ItemStatus.Find(1).Status + " (" + item.Price.ToString() + " kr)";
                statusLog.UserName = GetLoggedOnUser().FullName;
                ctx.ItemChangeLogs.Add(statusLog);
                ctx.SaveChanges();
            }
        }

        public void UpdateItem(int id, string description, int price, int sellerShare, int itemStatusId)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Item item = ctx.Items.Find(id);
            User loggedOnUser = GetLoggedOnUser();

            if (item != null)
            {
                //LOG CHANGES
                //Price
                if (item.Price != price)
                {
                    ItemChangeLog log = new ItemChangeLog();
                    log.TimeStamp = DateTime.Now;
                    log.ItemId = item.Id;
                    log.Log = "Prisändring (" + price.ToString() + " kr)";
                    log.UserName = loggedOnUser.FullName;
                    ctx.ItemChangeLogs.Add(log);
                }

                //ItemStaus
                if (item.ItemStatus.Id != itemStatusId)
                {
                    ItemChangeLog log = new ItemChangeLog();
                    log.TimeStamp = DateTime.Now;
                    log.ItemId = item.Id;
                    log.Log = ctx.ItemStatus.Find(itemStatusId).Status;
                    if (itemStatusId == 3)
                        log.Log += " (" + price.ToString() + " kr)";
                    if (itemStatusId == 5)
                        log.Log += " (" + item.SellersShare.ToString() + " kr)";
                    log.UserName = loggedOnUser.FullName;
                    ctx.ItemChangeLogs.Add(log);
                }

                item.Description = description;
                item.Price = price;
                item.SellersShare = sellerShare;
                item.ItemStatus = ctx.ItemStatus.Find(itemStatusId);
                ctx.SaveChanges();
            }
        }

        public void DeleteItem(int id)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Item item = ctx.Items.Find(id);
            if (item != null)
            {
                ItemChangeLog log = new ItemChangeLog();
                log.TimeStamp = DateTime.Now;
                log.ItemId = item.Id;
                log.Log = "Raderad";
                log.UserName = GetLoggedOnUser().FullName;
                ctx.ItemChangeLogs.Add(log);

                ctx.Items.Remove(item);
                ctx.SaveChanges();
            }
        }

        public void UpdateNumberOfLabels(int itemId, int numberOfLabels)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Item item = ctx.Items.Find(itemId);

            if (item != null)
            {
                item.NumberOfLabels = numberOfLabels;
                ctx.SaveChanges();
            }
        }

        public void UpdateItemStatus(int id, int statusId)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            Item item = ctx.Items.Find(id);
            User loggedOnUser = GetLoggedOnUser();

            if (item != null)
            {
                ItemStatus itemStatus = ctx.ItemStatus.Find(statusId);

                if (itemStatus != null)
                {
                    //ItemStaus
                    if (item.ItemStatus.Id != itemStatus.Id)
                    {
                        ItemChangeLog log = new ItemChangeLog();
                        log.TimeStamp = DateTime.Now;
                        log.ItemId = item.Id;
                        log.Log = ctx.ItemStatus.Find(itemStatus.Id).Status;
                        if (itemStatus.Id == 3)
                            log.Log += " (" + item.Price.ToString() + " kr)";
                        if (itemStatus.Id == 5)
                            log.Log += " (" + item.SellersShare.ToString() + " kr)";
                        log.UserName = loggedOnUser.FullName;
                        ctx.ItemChangeLogs.Add(log);
                    }

                    item.ItemStatus = itemStatus;
                    ctx.SaveChanges();
                }
            }
        }

        public bool CheckIfItemExists(int id)
        {
            SecondHandMarketContext ctx = new SecondHandMarketContext();
            bool exists = false;
            if (ctx.Items.Find(id) != null)
                exists = true;

            return exists;
        }
        
    }

    

    public enum ModalForm
    {
        None = 0,
        NewSalesman = 1,
        PrintOutSalesman = 2,
        PrintOutItem = 3
    }
}