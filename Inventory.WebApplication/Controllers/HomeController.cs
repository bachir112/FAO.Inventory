using Inventory.DataObjects.DTO;
using Inventory.DataObjects.EDM;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.WebApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Home"))
            {
                List<TransactionDTO> transactions = new List<TransactionDTO>();

                using (var db = new InventoryEntities())
                {
                    string userID = User.Identity.GetUserId();
                    AspNetUser user = db.AspNetUsers.FirstOrDefault(x => x.Id == userID);
                    if (user != null)
                    {
                        user.LastLogin = DateTime.Now;
                        db.SaveChanges();
                    }

                    //returnItems = db.Items.Where(x => x.Expandable != true && (x.AvailabilityStatusID == 2 || x.AvailabilityStatusID == 4)).Select(x => x).ToList();

                    Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                    List<string> nonExpandablesItems = db.Items.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.Expandable != true && (x.AvailabilityStatusID == 2 || x.AvailabilityStatusID == 4)).Select(x => x.Name).ToList(); 
                    transactions = db.Transactions
                                     .Where(x => nonExpandablesItems.Contains(x.ItemName) && (x.NewAvailabilityStatus == 2 || x.NewAvailabilityStatus == 4))
                                     .ToList()
                                     .Select(x => new TransactionDTO
                                     {
                                         Id = x.Id,
                                         ItemName = x.ItemName,
                                         ItemName_Arabic = x.ItemName_Arabic,
                                         Quantity = x.Quantity,
                                         NewAvailabilityStatusID = x.NewAvailabilityStatus,
                                         NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                                         NewAvailabilityStatus_Arabic = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status_Arabic : string.Empty,
                                         OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
                                         OldAvailabilityStatus_Arabic = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status_Arabic : string.Empty,
                                         StockKeeper = db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper) != null ? db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper).FullName : string.Empty,
                                         Description = x.Description,
                                         ToWhom = x.ToWhom,
                                         TransactionDate = x.TransactionDate
                                     }).ToList();
                }

                ViewBag.ReturnItems = transactions;

                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized","Home");
            }
        }

        public ActionResult Reports()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Reports"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        public ActionResult NotAuthorized()
        {
            string schoolCookieValue = Global.Global.GetSchoolCookieValue();
            if (schoolCookieValue == string.Empty || schoolCookieValue == null)
            {
                return RedirectToAction("LogOff", "Account");
            }
            else
            {
                ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
                return View();
            }
        }


        public ActionResult CategoriesPartial(string query)
        {
            List<CategoryDTO> categoriesList = new List<CategoryDTO>();

            int queryID = (query == "IN" ? 1 : query == "OUT" ? 2 : query == "TRASH" ? 3 : -1);

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                categoriesList = db.Categories
                                    .Select(x => new CategoryDTO
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Name_Arabic = x.Name_Arabic,
                                        Picture = x.Picture,
                                        ParentCategory = x.ParentCategory,
                                        Description_Arabic = x.Description_Arabic,
                                        Description = x.Description
                                    }).ToList();

                var itemsList = (from items in db.Items
                                 where (schoolID == 0 ? true : items.SchoolID == schoolID)
                                group items by items.CategoryID into itemsGroup
                                select new { 
                                    GroupID = itemsGroup.Key,
                                    ItemsNames = itemsGroup.Where(x => x.AvailabilityStatusID == (queryID == -1 ? x.AvailabilityStatusID : queryID)).Select(x => x.Name).Distinct().ToList(),
                                    TotalQuantityInGroup = itemsGroup.Where(x => x.AvailabilityStatusID == (queryID == -1 ? x.AvailabilityStatusID : queryID)).Count(),
                                    Items = itemsGroup.Where(x => x.AvailabilityStatusID == (queryID == -1 ? x.AvailabilityStatusID : queryID)).ToList()
                                }).ToList();

                foreach(var category in categoriesList)
                {
                    category.ItemTypeInCategoryCount = itemsList.Where(x => x.GroupID == category.Id).Select(x => x.ItemsNames).SelectMany(x => x).Distinct().Count();
                    //var ss = itemsList.Where(x => x.GroupID == category.Id).Select(x => x.ItemsNames).SelectMany(x => x).ToList();
                    category.ItemInCategoryCount = itemsList.Where(x => x.GroupID == category.Id).Select(x => x).SelectMany(x => x.Items).Count();
                }
            }

            ViewBag.Query = query;

            return View(categoriesList);
        }

        public ActionResult ItemsPartialDefault(Nullable<int> categoryID = null, string query = null)
        {
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            int queryID = (query == "IN" ? 1 : query == "OUT" ? 2 : query == "TRASH" ? 3 : -1);

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                where (schoolID == 0 ? true : item.SchoolID == schoolID)
                                && item.CategoryID == (categoryID == null ? item.CategoryID : categoryID)
                                && item.AvailabilityStatusID == (queryID == -1 ? item.AvailabilityStatusID : queryID)
                                && (queryID == 2 ? (item.Expandable != true) : true)
                                group item by new 
                                { 
                                    item.Name, 
                                    item.CategoryID,
                                    item.AvailabilityStatusID, 
                                    item.ExpiryDate, 
                                    item.UnitID, 
                                    item.UnitAmount, 
                                    item.ItemStatusID, 
                                    item.Description ,
                                    item.SchoolID
                                } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    ItemsIDs = string.Join(",", items.Select(x => x.Id).ToList()),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatus_Arabic = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status_Arabic,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description + " (" + items.Where(y => y.Description == x.Description).Select(y => y).Count().ToString() + ")").Distinct()),
                                    //Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ItemStatusID = items.Key.ItemStatusID,
                                    SchoolID = items.Key.SchoolID,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                ViewBag.CategoryName = categoryID == null ? null : db.Categories.First(x => x.Id == categoryID).Name;
                ViewBag.CategoryNameArabic = categoryID == null ? null : db.Categories.First(x => x.Id == categoryID).Name_Arabic;
            }

            ViewBag.CategoryID = categoryID;
            ViewBag.Query = query;

            return View(itemsInStock);
        }

        public ActionResult ItemsPartialPricing(Nullable<int> categoryID = null, Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where (schoolID == 0 ? true : item.SchoolID == schoolID)
                                && item.CategoryID == (categoryID == null ? item.CategoryID : categoryID)
                                && (fromDate == null ? true : item.ReceivedOn >= fromDate)
                                && (toDate == null ? true : item.ReceivedOn <= toDate)
                                group item by new { item.Name, item.ExpiryDate, item.UnitID, item.UnitAmount, item.SupplierID, item.ReceivedOn, item.Price, item.MaintenancePrice, item.SchoolID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    ItemsIDs = string.Join(",", items.Select(x => x.Id).ToList()),
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    Price = items.Key.Price,
                                    Supplier = suppliers.FirstOrDefault(x => x.Id == items.Key.SupplierID)?.Supplier1,
                                    Quantity = items.Count(),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    MaintenancePrice = items.Key.MaintenancePrice,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ReceivedOn = items.Key.ReceivedOn,
                                    SchoolID = items.Key.SchoolID,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID)?.Name
                                }).ToList();
            }
            
            return View(itemsInStock);
        }

        public ActionResult ItemsPartialApproval(Nullable<int> categoryID = null, Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where (schoolID == 0 ? true : item.SchoolID == schoolID)
                                && item.CategoryID == (categoryID == null ? item.CategoryID : categoryID)
                                && (fromDate == null ? true : item.ReceivedOn >= fromDate)
                                && (toDate == null ? true : item.ReceivedOn <= toDate)
                                && item.AvailabilityStatusID == 1002
                                && item.PendingTransferApproval == "waiting"
                                group item by new { item.Name, item.ExpiryDate, item.UnitID, item.UnitAmount, item.SupplierID, item.ReceivedOn, item.Description, item.SchoolID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    ItemsIDs = string.Join(",", items.Select(x => x.Id).ToList()),
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    Description = items.Key.Description,
                                    Supplier = suppliers.FirstOrDefault(x => x.Id == items.Key.SupplierID)?.Supplier1,
                                    Quantity = items.Count(),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ReceivedOn = items.Key.ReceivedOn,
                                    SchoolID = items.Key.SchoolID,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID)?.Name
                                }).ToList();
            }

            return View(itemsInStock);
        }


        public ActionResult Transactions()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Transactions"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        public ActionResult TransactionsIntoStock()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                ViewBag.ToWhom = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.ToWhom != null && x.ToWhom.Trim() != string.Empty)
                                                .Select(x => x.ToWhom)
                                                .Distinct()
                                                .ToList();

                ViewBag.Description = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.Description != null && x.Description.Trim() != string.Empty)
                                                .Select(x => x.Description)
                                                .Distinct()
                                                .ToList();

                ViewBag.AvailabilityStatus = db.AvailabilityStatus.Where(x => x.Id != 2).ToList();
            }

            return View();
        }

        public ActionResult TransactionsOutOfStock()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Reports"))
            {
                using (var db = new InventoryEntities())
                {
                    string userID = User.Identity.GetUserId();
                    Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                    ViewBag.ToWhom = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.ToWhom != null && x.ToWhom.Trim() != string.Empty)
                                                    .Select(x => x.ToWhom)
                                                    .Distinct()
                                                    .ToList();

                    ViewBag.Description = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.Description != null && x.Description.Trim() != string.Empty)
                                                    .Select(x => x.Description)
                                                    .Distinct()
                                                    .ToList();

                    ViewBag.AvailabilityStatus = db.AvailabilityStatus.Where(x => x.Id != 1).ToList();
                }

                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        public ActionResult Deteriorated()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Deteriorated"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            //return View();
        }

        public ActionResult RecentTransactionsPartial()
        {
            List<TransactionDTO> result = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                List<Transaction> transactions = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                result = transactions.Select(x => new TransactionDTO
                {
                    Id = x.Id,
                    ItemName = x.ItemName,
                    ItemName_Arabic = x.ItemName_Arabic,
                    Quantity = x.Quantity,
                    Unit = db.Units.FirstOrDefault(y => y.Id == x.UnitID) != null ? db.Units.FirstOrDefault(y => y.Id == x.UnitID).Name : string.Empty,
                    UnitAmount = x.UnitAmount,
                    NewAvailabilityStatusID = x.NewAvailabilityStatus,
                    NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                    NewAvailabilityStatus_Arabic = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status_Arabic : string.Empty,
                    OldAvailabilityStatusID = x.OldAvailabilityStatus,
                    OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
                    OldAvailabilityStatus_Arabic = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status_Arabic : string.Empty,
                    StockKeeper = db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper) != null ? db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper).UserName : string.Empty,
                    Description = x.Description,
                    ToWhom = x.ToWhom,
                    TransactionDate = x.TransactionDate
                }).OrderByDescending(x => x.TransactionDate).ToList();
            }

            return View(result);
        }
        
        public ActionResult TransactionsHistory()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "TransactionsHistory"))
            {
                List<TransactionDTO> result = new List<TransactionDTO>();

                using (var db = new InventoryEntities())
                {
                    string userID = User.Identity.GetUserId();
                    Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                    List<Transaction> transactions = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                    result = transactions.Select(x => new TransactionDTO
                    {
                        Id = x.Id,
                        ItemName = x.ItemName,
                        ItemName_Arabic = x.ItemName_Arabic,
                        Quantity = x.Quantity,
                        NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                        NewAvailabilityStatus_Arabic = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status_Arabic : string.Empty,
                        OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
                        OldAvailabilityStatus_Arabic = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status_Arabic : string.Empty,
                        StockKeeper = db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper) != null ? db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper).FullName : string.Empty,
                        Description = x.Description,
                        ToWhom = x.ToWhom,
                        TransactionDate = x.TransactionDate
                    }).ToList();
                }

                return View(result);
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }

        }

        public JsonResult ChangeItemDescription(int itemID, string description)
        {
            string result = "success";

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                try
                {
                    var item = db.Items.FirstOrDefault(x => x.SchoolID == x.SchoolID && x.Id == itemID);
                    if(item != null)
                    {
                        item.Description = description;
                        item.ModifiedOn = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                catch
                {
                    result = "error";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssignItems(int quantity, 
            int AvailabilityStatusID, 
            string LocationInStock, 
            string Description,
            string ToWhom, 
            IEnumerable<Dictionary<string, object>> selectedItems)
        {
            List<string> result = new List<string>();

            var selectedItemsJSON = JsonConvert.SerializeObject(selectedItems);
            List<ItemsGroupedDTO> selectedItemsList = JsonConvert.DeserializeObject<List<ItemsGroupedDTO>>(selectedItemsJSON);

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                foreach (var item in selectedItemsList)
                {
                    try
                    {
                        string removeSection = item.Description
                                               .Substring(item.Description.LastIndexOf("("),
                                                          item.Description.LastIndexOf(")") - item.Description.LastIndexOf("(") + 1
                                                         );

                        item.Description = item.Description.Replace(removeSection, string.Empty);
                    }
                    catch (Exception ex)
                    {

                    }

                    var itemInDB = db.Items.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) &&
                                                    x.Name == item.Name && 
                                                    x.AvailabilityStatusID == item.AvailabilityStatusID && 
                                                    x.ItemStatusID == item.ItemStatusID &&
                                                    x.ExpiryDate == item.ExpiryDate && 
                                                    //x.ReceivedOn == item.ReceivedOn &&
                                                    x.UnitID == item.UnitID &&
                                                    x.UnitAmount == item.UnitAmount &&
                                                    (
                                                        x.LocationInStock.Trim() == LocationInStock.Trim()
                                                        ||
                                                        x.LocationInStock == (LocationInStock == string.Empty ? null : string.Empty)
                                                    )
                                                    &&
                                                    (
                                                        x.Description.Trim() == (item.Description == string.Empty ? null : item.Description.Trim())
                                                        ||
                                                        x.Description.Trim() == (item.Description == null ? string.Empty : item.Description.Trim())
                                                        ||
                                                        x.Description.Trim() == item.Description.Trim()
                                                    )
                                                    ).Select(x => x).Take(quantity).ToList();

                    if(itemInDB.Count() < quantity)
                    {
                        throw new NullReferenceException(Global.Translation.GetStringValue("QuantityError"));
                    }

                    itemInDB = itemInDB.Take(quantity).ToList();

                    itemInDB.ForEach(x => x.AvailabilityStatusID = AvailabilityStatusID);
                    itemInDB.ForEach(x => x.Description = Description);
                    itemInDB.ForEach(x => x.ModifiedOn = DateTime.Now);

                    if (AvailabilityStatusID == 1002)
                    {
                        itemInDB.ForEach(x => x.PendingTransferApproval = "waiting");
                    }

                    Transaction newTransaction = new Transaction();
                    newTransaction.ItemName = itemInDB.FirstOrDefault() != null ? itemInDB.First().Name : string.Empty;
                    newTransaction.ItemName_Arabic = itemInDB.FirstOrDefault() != null ? itemInDB.First().Name_Arabic : string.Empty;
                    newTransaction.OldAvailabilityStatus = item.AvailabilityStatusID;
                    newTransaction.NewAvailabilityStatus = AvailabilityStatusID;
                    newTransaction.StockKeeper = User.Identity.GetUserId();
                    newTransaction.Quantity = quantity;
                    newTransaction.ToWhom = ToWhom;
                    newTransaction.Description = Description;
                    newTransaction.UnitID = item.UnitID;
                    newTransaction.UnitAmount = item.UnitAmount;
                    newTransaction.TransactionDate = DateTime.Now;
                    newTransaction.SchoolID = item.SchoolID;

                    db.Transactions.Add(newTransaction);

                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemsInCategory(Nullable<int> categoryID = null)
        {
            List<SearchItemsDTO> result = new List<SearchItemsDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                List<SearchItemsDTO>  items = db.Items
                                                .Where(x => ((schoolID == 0 ? true : x.SchoolID == schoolID)) && (categoryID == null) ? true : x.CategoryID == categoryID)
                                                .Select(x => new SearchItemsDTO
                                                {
                                                    EnglishName = x.Name == null ? string.Empty : x.Name,
                                                    ArabicName = x.Name_Arabic == null ? string.Empty : x.Name_Arabic
                                                }).ToList();

                List<SearchItemsDTO> searchableItems = db.ItemsSearchValues
                                                         .Where(x => (categoryID == null) ? true : x.CategoryID == categoryID)
                                                         .Select(x => new SearchItemsDTO
                                                         {
                                                             EnglishName = x.ItemName == null ? string.Empty : x.ItemName,
                                                             ArabicName = x.ItemName_Arabic == null ? string.Empty : x.ItemName_Arabic
                                                         }).ToList();

                result.AddRange(searchableItems);
            }

            List<SearchItemsDTO>  distinctResult = result.Distinct().Select(x => x).ToList();

            return Json(distinctResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddItemToSearchble(string itemName, string itemName_Arabic, int categoryID)
        {
            string result = "error";

            try
            {
                using (var db = new InventoryEntities())
                {
                    ItemsSearchValue newItem = new ItemsSearchValue();
                    newItem.ItemName = itemName;
                    newItem.ItemName_Arabic = itemName_Arabic;
                    newItem.CategoryID = categoryID;

                    db.ItemsSearchValues.Add(newItem);
                    db.SaveChanges();
                }
            }
            catch
            {

            }

            result = "success";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}