using Inventory.DataObjects.DTO;
using Inventory.DataObjects.EDM;
using Inventory.WebApplication.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Inventory.WebApplication.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Reports"))
            {
                using (var db = new InventoryEntities())
                {
                    string userID = User.Identity.GetUserId();
                    Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                    ViewBag.ItemsNames = db.Items.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).Select(x => new ItemNames
                                                              {
                                                                  ItemName = x.Name,
                                                                  ItemName_Arabic = x.Name_Arabic
                                                              }).Distinct().ToList();

                    ViewBag.AvailabilityStatus = db.AvailabilityStatus.ToList();
                    ViewBag.Users = db.AspNetUsers.ToList();
                }
                
                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        public JsonResult CreateReportSettings(int ReportID, string ReportName, string ReceivedByUsers, 
            bool DailyBasis, bool WeeklyBasis, bool MonthlyBasis, bool YearlyBasis)//, string SpecificDates)
        {
            string result = "error";

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                ReportSetting reportSettings = db.ReportSettings.FirstOrDefault(x => x.ReportID == ReportID && (schoolID == 0 ? true : x.SchoolID == schoolID));

                if(reportSettings == null)
                {
                    ReportSetting newSettings = new ReportSetting();
                    newSettings.ReportID = ReportID;
                    newSettings.ReportName = ReportName;
                    newSettings.ReceivedByUsers = ReceivedByUsers;
                    newSettings.DailyBasis = DailyBasis;
                    newSettings.WeeklyBasis = WeeklyBasis;
                    newSettings.MonthlyBasis = MonthlyBasis;
                    newSettings.YearlyBasis = YearlyBasis;

                    db.ReportSettings.Add(newSettings);
                    db.SaveChanges();
                }
                else
                {
                    reportSettings.ReportID = ReportID;
                    reportSettings.ReportName = ReportName;
                    reportSettings.ReceivedByUsers = ReceivedByUsers;
                    reportSettings.DailyBasis = DailyBasis;
                    reportSettings.WeeklyBasis = WeeklyBasis;
                    reportSettings.MonthlyBasis = MonthlyBasis;
                    reportSettings.YearlyBasis = YearlyBasis;

                    db.SaveChanges();
                }

                result = "success";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateQuery(string itemName, int reportID,
            Nullable<int> itemAvailability,
            Nullable<int> minQuantity, Nullable<int> maxQuantity,
            Nullable<int> minPrice, Nullable<int> maxPrice)
        {
            string result = "error";

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                ReportQuery newReportQuery = new ReportQuery();
                newReportQuery.ReportID = reportID;
                newReportQuery.ItemName = itemName;
                newReportQuery.AvailabilityStatusID = itemAvailability;
                newReportQuery.MinimumPrice = minPrice;
                newReportQuery.MaximumPrice = maxPrice;
                newReportQuery.MinimumQuantity = minQuantity;
                newReportQuery.MaximumQuantity = maxQuantity;
                newReportQuery.SchoolID = schoolID;

                db.ReportQueries.Add(newReportQuery);
                db.SaveChanges();

                result = "success";
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteQuery(int queryID)
        {
            string result = string.Empty;

            try
            {
                using (var db = new InventoryEntities())
                {
                    string userID = User.Identity.GetUserId();
                    Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                    ReportQuery query = db.ReportQueries.First(x => x.Id == queryID && (schoolID == 0 ? true : x.SchoolID == schoolID));
                    db.ReportQueries.Remove(query);
                    db.SaveChanges();

                    result = "success";
                }

            }
            catch(Exception ex)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportSettings(int reportID)
        {
            Dictionary<string, object> reportSettingsDict = new Dictionary<string, object>();

            ReportSetting reportSettings = new ReportSetting();
            List<ReportQuery> listOfReportQuery = new List<ReportQuery>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                reportSettings = db.ReportSettings.FirstOrDefault(x => x.ReportID == reportID && (schoolID == 0 ? true : x.SchoolID == schoolID));
                listOfReportQuery = db.ReportQueries.Where(x => x.ReportID == reportID && (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();

                reportSettingsDict.Add("reportSettings", reportSettings);
                reportSettingsDict.Add("listOfReportQuery", listOfReportQuery);
            }

            return Json(reportSettingsDict, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InventoryGeneralReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;

                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                if(fromDate == null || toDate == null)
                {
                    itemsInStock = (from item in db.Items
                                    where item.SchoolID == schoolID
                                    group item by new
                                    {
                                        item.Name,
                                        item.AvailabilityStatusID,
                                        item.ExpiryDate,
                                        item.Expandable,
                                        item.UnitID,
                                        item.UnitAmount,
                                        item.ItemStatusID,
                                        item.ReceivedOn,
                                        item.CategoryID,
                                        item.SupplierID
                                    } into items
                                    select items).AsEnumerable().Select(
                                    items => new ItemsGroupedDTO()
                                    {
                                        GroupedId = items.FirstOrDefault().Id,
                                        Name = items.Key.Name,
                                        Name_Arabic = items.First().Name_Arabic,
                                        ReceivedOn = items.Key.ReceivedOn,
                                        CategoryID = items.Key.CategoryID,
                                        Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                        Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                        Supplier = suppliers.FirstOrDefault(x => x.Id == items.Key.SupplierID)?.Supplier1,
                                        AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                        AvailabilityStatus_Arabic = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status_Arabic,
                                        AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                        Expandable = items.Key.Expandable,
                                        Quantity = items.Count(),
                                        LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                        Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                        ExpiryDate = items.Key.ExpiryDate,
                                        UnitID = items.Key.UnitID,
                                        UnitAmount = items.Key.UnitAmount,
                                        ItemStatusID = items.Key.ItemStatusID,
                                        Price = items.Sum(x => x.Price),
                                        Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                    }).ToList();
                }
                else
                {
                    itemsInStock = (from item in db.Items
                                    where item.ReceivedOn >= fromDate && item.ReceivedOn <= toDate && item.SchoolID == schoolID
                                    group item by new
                                    {
                                        item.Name,
                                        item.AvailabilityStatusID,
                                        item.ExpiryDate,
                                        item.Expandable,
                                        item.UnitID,
                                        item.UnitAmount,
                                        item.ItemStatusID,
                                        item.ReceivedOn,
                                        item.CategoryID,
                                        item.SupplierID
                                    } into items
                                    select items).AsEnumerable().Select(
                                    items => new ItemsGroupedDTO()
                                    {
                                        GroupedId = items.FirstOrDefault().Id,
                                        Name = items.Key.Name,
                                        Name_Arabic = items.First().Name_Arabic,
                                        ReceivedOn = items.Key.ReceivedOn,
                                        CategoryID = items.Key.CategoryID,
                                        Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                        Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                        Supplier = suppliers.FirstOrDefault(x => x.Id == items.Key.SupplierID)?.Supplier1,
                                        AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                        AvailabilityStatus_Arabic = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status_Arabic,
                                        AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                        Expandable = items.Key.Expandable,
                                        Quantity = items.Count(),
                                        LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                        Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                        ExpiryDate = items.Key.ExpiryDate,
                                        UnitID = items.Key.UnitID,
                                        UnitAmount = items.Key.UnitAmount,
                                        ItemStatusID = items.Key.ItemStatusID,
                                        Price = items.Sum(x => x.Price),
                                        Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                    }).ToList();
                }


                List<Transaction> transactions = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).Select(x => x).OrderByDescending(x => x.TransactionDate).ToList();

                foreach(var item in itemsInStock.Where(x => x.AvailabilityStatusID != 1).Select(x => x).ToList())
                {
                    item.DateOn = transactions.FirstOrDefault(x => x.ItemName == item.Name)?.TransactionDate;
                }
            }

            return View(itemsInStock);
        }
        
        public void InventoryGeneralReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.SchoolID == schoolID
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ItemStatusID = items.Key.ItemStatusID,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInStock);
                string emailBody = "Please find attached to this email a copy of the Inventory General Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    Global.Global.sendEmail("Inventory General Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        Global.Global.sendEmail("Inventory General Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult ItemsInReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.AvailabilityStatusID == 1 && item.SchoolID == schoolID
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Description, item.ReceivedOn } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    ReceivedOn = items.Key.ReceivedOn,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Description = items.Key.Description,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();
            }

            return View(itemsInStock);
        }

        public void ItemsInReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.AvailabilityStatusID == 1 && item.SchoolID == schoolID
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Description } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Description = items.Key.Description,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInStock);
                string emailBody = "Please find attached to this email a copy of the Items In Report";

                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Items In Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Items In Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult SearchForNonConsumableReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<TransactionDTO> result = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                List<string> nonExpandables = db.Items.Where(x => x.Expandable != true && (schoolID == 0 ? true : x.SchoolID == schoolID)).Select(x => x.Name).ToList(); 
                List<Transaction> transactions = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).Select(x => x).ToList();
                if(fromDate == null || toDate == null)
                {
                    result = transactions.Where(x => nonExpandables.Contains(x.ItemName)).Select(x => new TransactionDTO
                    {
                        Id = x.Id,
                        ItemName = x.ItemName,
                        ItemName_Arabic = x.ItemName_Arabic,
                        Quantity = x.Quantity,
                        NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                        OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
                        StockKeeper = db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper) != null ? db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper).FullName : string.Empty,
                        Description = x.Description,
                        ToWhom = x.ToWhom,
                        TransactionDate = x.TransactionDate
                    }).ToList();
                }
                else
                {
                    result = transactions
                        .Where(x => nonExpandables.Contains(x.ItemName)
                        && x.TransactionDate >= fromDate && x.TransactionDate <= toDate)
                        .Select(x => new TransactionDTO
                    {
                        Id = x.Id,
                        ItemName = x.ItemName,
                        ItemName_Arabic = x.ItemName_Arabic,
                        Quantity = x.Quantity,
                        NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                        OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
                        StockKeeper = db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper) != null ? db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper).FullName : string.Empty,
                        Description = x.Description,
                        ToWhom = x.ToWhom,
                        TransactionDate = x.TransactionDate
                    }).ToList();
                }
            }

            return View(result);
        }

        public void SearchForNonConsumableReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<TransactionDTO> result = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<Transaction> transactions = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).Select(x => x).ToList();
                result = transactions.Select(x => new TransactionDTO
                {
                    Id = x.Id,
                    ItemName = x.ItemName,
                    Quantity = x.Quantity,
                    NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                    OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
                    StockKeeper = db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper) != null ? db.AspNetUsers.FirstOrDefault(y => y.Id == x.StockKeeper).FullName : string.Empty,
                    Description = x.Description,
                    ToWhom = x.ToWhom,
                    TransactionDate = x.TransactionDate
                }).ToList();

                Global.Global.ExportDataSetToExcel(result);
                string emailBody = "Please find attached to this email a copy of the Search For Non Consumable Report";

                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Search For Non Consumable Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Search For Non Consumable Report", emailBody, email);
                    }
                }
            }
        }

        public ActionResult DailyReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<TransactionsReminder> transactionsReminders = new List<TransactionsReminder>();

            using (var db = new InventoryEntities())
            {
                string userID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == userID)?.SchoolID;
                transactionsReminders.AddRange(db.TransactionsReminders.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList());
            }

            return View(transactionsReminders);
        }

        public void DailyReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<TransactionsReminder> transactionsReminders = new List<TransactionsReminder>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                transactionsReminders.AddRange(db.TransactionsReminders.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList());

                Global.Global.ExportDataSetToExcel(transactionsReminders);
                string emailBody = "Please find attached to this email a copy of the Daily Report";

                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Daily Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach(var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Daily Report", emailBody, email);
                    }
                }
            }
        }

        public ActionResult ConsumableItemsReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                if(fromDate == null || toDate == null)
                {
                    List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                          where (item.SchoolID == schoolID) && (item.Expandable == true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                          group item by new { item.Name, item.UnitID, item.UnitAmount, item.CategoryID, item.Expandable } into items
                                                          select items).AsEnumerable().Select(
                                                          items => new ItemsGroupedDTO()
                                                          {
                                                              GroupedId = items.FirstOrDefault().Id,
                                                              Name = items.Key.Name,
                                                              Name_Arabic = items.First().Name_Arabic,
                                                              Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                                              Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                                              Quantity = items.Count(),
                                                              Expandable = items.Key.Expandable,
                                                              LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                                              Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                                              UnitID = items.Key.UnitID,
                                                              UnitAmount = items.Key.UnitAmount,
                                                              Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                                          }).ToList();

                    itemsInReportQuery = (from item in itemsInStock
                                          join query in db.ReportQueries on item.Name equals query.ItemName into queries
                                          from q in queries.DefaultIfEmpty()
                                          select new ItemsInReportQuery()
                                          {
                                              Name = item.Name,
                                              Name_Arabic = item.Name_Arabic,
                                              Category = item.Category,
                                              Category_Arabic = item.Category_Arabic,
                                              UnitID = item.UnitID,
                                              UnitAmount = item.UnitAmount,
                                              Quantity = item.Quantity,
                                              Expandable = item.Expandable,
                                              AvailabilityStatusID = item.AvailabilityStatusID,
                                              MinimumQuantity = q != null ? q.MinimumQuantity : null,
                                              MaximumQuantity = q != null ? q.MaximumQuantity : null,
                                              Description = item.Description,
                                              AvailabilityStatus = item.AvailabilityStatus,
                                              Unit = item.Unit
                                          }).ToList();
                }
                else
                {
                    List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                          where 
                                                                (item.SchoolID == schoolID) &&
                                                                (item.Expandable == true) && 
                                                                (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2) &&
                                                                (item.ModifiedOn >= fromDate && item.ModifiedOn <= toDate)
                                                          group item by new { item.Name, item.UnitID, item.UnitAmount, item.CategoryID, item.Expandable } into items
                                                          select items).AsEnumerable().Select(
                                                          items => new ItemsGroupedDTO()
                                                          {
                                                              GroupedId = items.FirstOrDefault().Id,
                                                              Name = items.Key.Name,
                                                              Name_Arabic = items.First().Name_Arabic,
                                                              Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                                              Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                                              Quantity = items.Count(),
                                                              Expandable = items.Key.Expandable,
                                                              LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                                              Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                                              UnitID = items.Key.UnitID,
                                                              UnitAmount = items.Key.UnitAmount,
                                                              Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                                          }).ToList();

                    itemsInReportQuery = (from item in itemsInStock
                                          join query in db.ReportQueries on item.Name equals query.ItemName into queries
                                          from q in queries.DefaultIfEmpty()
                                          select new ItemsInReportQuery()
                                          {
                                              Name = item.Name,
                                              Name_Arabic = item.Name_Arabic,
                                              Category = item.Category,
                                              Category_Arabic = item.Category_Arabic,
                                              UnitID = item.UnitID,
                                              UnitAmount = item.UnitAmount,
                                              Quantity = item.Quantity,
                                              Expandable = item.Expandable,
                                              AvailabilityStatusID = item.AvailabilityStatusID,
                                              MinimumQuantity = q != null ? q.MinimumQuantity : null,
                                              MaximumQuantity = q != null ? q.MaximumQuantity : null,
                                              Description = item.Description,
                                              AvailabilityStatus = item.AvailabilityStatus,
                                              Unit = item.Unit
                                          }).ToList();
                }

            }

            return View(itemsInReportQuery);
        }

        public void ConsumableItemsReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.SchoolID == schoolID) && (item.Expandable == true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
                                                          Name_Arabic = items.First().Name_Arabic,
                                                          Quantity = items.Count(),
                                                          LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                                          Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                                          UnitID = items.Key.UnitID,
                                                          UnitAmount = items.Key.UnitAmount,
                                                          Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                                      }).ToList();

                itemsInReportQuery = (from item in itemsInStock
                                      join query in db.ReportQueries on (item.Name + item.UnitAmount + item.Unit) equals query.ItemName into queries
                                      from q in queries.DefaultIfEmpty()
                                      select new ItemsInReportQuery()
                                      {
                                          Name = item.Name,
                                          Name_Arabic = item.Name_Arabic,
                                          UnitID = item.UnitID,
                                          UnitAmount = item.UnitAmount,
                                          Quantity = item.Quantity,
                                          AvailabilityStatusID = item.AvailabilityStatusID,
                                          MinimumQuantity = q != null ? q.MinimumQuantity : null,
                                          MaximumQuantity = q != null ? q.MaximumQuantity : null,
                                          Description = item.Description,
                                          AvailabilityStatus = item.AvailabilityStatus,
                                          Unit = item.Unit
                                      }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInReportQuery);
                string emailBody = "Please find attached to this email a copy of the Consumable Items Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Consumable Items Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Consumable Items Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult NonConsumableItemsReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.SchoolID == schoolID) && (item.Expandable != true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
                                                          Name_Arabic = items.First().Name_Arabic,
                                                          Quantity = items.Count(),
                                                          LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                                          Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                                          UnitID = items.Key.UnitID,
                                                          UnitAmount = items.Key.UnitAmount,
                                                          Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                                      }).ToList();

                itemsInReportQuery = (from item in itemsInStock
                                      join query in db.ReportQueries on item.Name equals query.ItemName into queries
                                      from q in queries.DefaultIfEmpty()
                                      select new ItemsInReportQuery()
                                      {
                                          Name = item.Name,
                                          Name_Arabic = item.Name_Arabic,
                                          UnitID = item.UnitID,
                                          UnitAmount = item.UnitAmount,
                                          Quantity = item.Quantity,
                                          AvailabilityStatusID = item.AvailabilityStatusID,
                                          MinimumQuantity = q != null ? q.MinimumQuantity : null,
                                          MaximumQuantity = q != null ? q.MaximumQuantity : null,
                                          Description = item.Description,
                                          AvailabilityStatus = item.AvailabilityStatus,
                                          Unit = item.Unit
                                      }).ToList();
            }

            return View(itemsInReportQuery);
        }

        public void NonConsumableItemsReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.SchoolID == schoolID) && (item.Expandable != true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
                                                          Name_Arabic = items.First().Name_Arabic,
                                                          Quantity = items.Count(),
                                                          LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                                          Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                                          UnitID = items.Key.UnitID,
                                                          UnitAmount = items.Key.UnitAmount,
                                                          Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                                      }).ToList();

                itemsInReportQuery = (from item in itemsInStock
                                      join query in db.ReportQueries on (item.Name + item.UnitAmount + item.Unit) equals query.ItemName into queries
                                      from q in queries.DefaultIfEmpty()
                                      select new ItemsInReportQuery()
                                      {
                                          Name = item.Name,
                                          Name_Arabic = item.Name_Arabic,
                                          UnitID = item.UnitID,
                                          UnitAmount = item.UnitAmount,
                                          Quantity = item.Quantity,
                                          AvailabilityStatusID = item.AvailabilityStatusID,
                                          MinimumQuantity = q != null ? q.MinimumQuantity : null,
                                          MaximumQuantity = q != null ? q.MaximumQuantity : null,
                                          Description = item.Description,
                                          AvailabilityStatus = item.AvailabilityStatus,
                                          Unit = item.Unit
                                      }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInReportQuery);
                string emailBody = "Please find attached to this email a copy of the Non-Consumable Items Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Non Consumable Items Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Non Consumable Items Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult FullInventoryGeneralReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                if(fromDate == null || toDate == null)
                {
                    itemsInStock = (from item in db.Items
                                    where item.SchoolID == schoolID
                                    group item by new
                                    {
                                        item.Name,
                                        item.AvailabilityStatusID,
                                        item.ExpiryDate,
                                        item.UnitID,
                                        item.UnitAmount,
                                        item.ItemStatusID,
                                        item.Price,
                                        item.CategoryID,
                                        item.Expandable
                                    } into items
                                    select items).AsEnumerable().Select(
                                    items => new ItemsGroupedDTO()
                                    {
                                        GroupedId = items.FirstOrDefault().Id,
                                        Name = items.Key.Name,
                                        Name_Arabic = items.First().Name_Arabic,
                                        AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                        AvailabilityStatus_Arabic = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status_Arabic,
                                        AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                        Quantity = items.Count(),
                                        Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                        Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                        LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                        Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                        ExpiryDate = items.Key.ExpiryDate,
                                        Expandable = items.Key.Expandable,
                                        UnitID = items.Key.UnitID,
                                        UnitAmount = items.Key.UnitAmount,
                                        ItemStatusID = items.Key.ItemStatusID,
                                        Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name,
                                        Price = items.Key.Price,
                                        TotalPrice = items.Key.Price * items.Count()
                                    }).ToList();
                }
                else
                {
                    itemsInStock = (from item in db.Items
                                    where item.SchoolID == schoolID
                                    where item.ModifiedOn >= fromDate && item.ModifiedOn <= toDate
                                    group item by new
                                    {
                                        item.Name,
                                        item.AvailabilityStatusID,
                                        item.ExpiryDate,
                                        item.UnitID,
                                        item.UnitAmount,
                                        item.ItemStatusID,
                                        item.Price,
                                        item.CategoryID,
                                        item.Expandable
                                    } into items
                                    select items).AsEnumerable().Select(
                                    items => new ItemsGroupedDTO()
                                    {
                                        GroupedId = items.FirstOrDefault().Id,
                                        Name = items.Key.Name,
                                        Name_Arabic = items.First().Name_Arabic,
                                        AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                        AvailabilityStatus_Arabic = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status_Arabic,
                                        AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                        Quantity = items.Count(),
                                        Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                        Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                        LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                        Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                        ExpiryDate = items.Key.ExpiryDate,
                                        Expandable = items.Key.Expandable,
                                        UnitID = items.Key.UnitID,
                                        UnitAmount = items.Key.UnitAmount,
                                        ItemStatusID = items.Key.ItemStatusID,
                                        Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name,
                                        Price = items.Key.Price,
                                        TotalPrice = items.Key.Price * items.Count()
                                    }).ToList();
                }

            }

            return View(itemsInStock);
        }

        public void FullInventoryGeneralReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                where item.SchoolID == schoolID
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ItemStatusID = items.Key.ItemStatusID,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name,
                                    Price = items.Key.Price,
                                    TotalPrice = items.Key.Price * items.Count()
                                }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInStock);
                string emailBody = "Please find attached to this email a copy of the Full Inventory General Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Full Inventory General Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Full Inventory General Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult BudgetLineStatementOfAccountReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                where item.SchoolID == schoolID
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name,
                                    Price = items.Key.Price,
                                    TotalPrice = items.Key.Price * items.Count()
                                }).ToList();
            }

            return View(itemsInStock);
        }

        public void BudgetLineStatementOfAccountReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                where item.SchoolID == schoolID
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name,
                                    Price = items.Key.Price,
                                    TotalPrice = items.Key.Price * items.Count()
                                }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInStock);
                string emailBody = "Please find attached to this email a copy of the BudgetLine Statement Of Account Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("BudgetLine Statement Of Account Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("BudgetLine Statement Of Account Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult QuantityReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();
            List<TransactionDTO> transactions = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                if(fromDate == null || toDate == null)
                {
                    transactions = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID))
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
                else
                {

                    transactions = db.Transactions
                                     .Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.TransactionDate >= fromDate && x.TransactionDate <= toDate)
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

                foreach(var t in transactions)
                {
                    Item item = db.Items.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).FirstOrDefault(x => x.Name == t.ItemName);
                    if(item != null)
                    {
                        Category category = db.Categories.FirstOrDefault(x => x.Id == item.CategoryID);
                        t.Category = category.Name;
                        t.Category_Arabic = category.Name_Arabic;
                    }
                    t.QuantityAvailable = db.Items.Where(x => x.AvailabilityStatusID != 3 && x.AvailabilityStatusID != 1002).Select(x => x).Count();
                }

            }

            return View(transactions);
        }

        public void QuantityReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                where item.SchoolID == schoolID
                                group item by new { item.Name, item.UnitID, item.UnitAmount, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Name_Arabic = items.First().Name_Arabic,
                                    Quantity = items.Count(),
                                    QuantityIn = items.Where(x => x.AvailabilityStatusID == 1 || x.AvailabilityStatusID == 2).Count(),
                                    QuantityOut = items.Where(x => x.AvailabilityStatusID == 3).Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    Category_Arabic = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name_Arabic,
                                    CategoryID = items.Key.CategoryID,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInStock);
                string emailBody = "Please find attached to this email a copy of the Quantity Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("Quantity Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("Quantity Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult SchoolTransferReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<TransactionDTO> itemsInStock = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                if(fromDate == null || toDate == null)
                {
                    itemsInStock = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.NewAvailabilityStatus == 1002 && (schoolID == 0 ? true : x.SchoolID == schoolID)).Select(
                                    items => new TransactionDTO()
                                    {
                                        Id = items.Id,
                                        Description = items.Description,
                                        ItemName = items.ItemName,
                                        Quantity = items.Quantity,
                                        StockKeeper = items.StockKeeper,
                                        ToWhom = items.ToWhom,
                                        TransactionDate = items.TransactionDate,
                                        UnitAmount = items.UnitAmount
                                    }).ToList();
                }
                else
                {
                    itemsInStock = db.Transactions
                                    .Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.NewAvailabilityStatus == 1002 && 
                                                (x.TransactionDate >= fromDate && x.TransactionDate <= toDate)
                                                )
                                    .Select(
                                    items => new TransactionDTO()
                                    {
                                        Id = items.Id,
                                        Description = items.Description,
                                        ItemName = items.ItemName,
                                        Quantity = items.Quantity,
                                        StockKeeper = items.StockKeeper,
                                        ToWhom = items.ToWhom,
                                        TransactionDate = items.TransactionDate,
                                        UnitAmount = items.UnitAmount
                                    }).ToList();
                }

            }

            return View(itemsInStock);
        }

        public void SchoolTransferReport_Email(string email, string usersListID = null, string queryReport = null)
        {
            string sendToEmail = string.Empty;
            List<TransactionDTO> itemsInStock = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = db.Transactions.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.NewAvailabilityStatus == 1002).Select(
                                items => new TransactionDTO()
                                {
                                    Id = items.Id,
                                    Description = items.Description,
                                    ItemName = items.ItemName,
                                    //NewAvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.NewAvailabilityStatus).Status,
                                    //OldAvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.OldAvailabilityStatus).Status,
                                    Quantity = items.Quantity,
                                    StockKeeper = items.StockKeeper,
                                    ToWhom = items.ToWhom,
                                    TransactionDate = items.TransactionDate,
                                    //Unit = units.FirstOrDefault(x => x.Id == items.UnitID).Name,
                                    UnitAmount = items.UnitAmount
                                }).ToList();

                Global.Global.ExportDataSetToExcel(itemsInStock);
                string emailBody = "Please find attached to this email a copy of the School Transfer Report";
                if (usersListID == null)
                {
                    string userID = User.Identity.GetUserId();
                    sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                    Global.Global.sendEmail("School Transfer Report", emailBody, email);
                }
                else
                {
                    emailBody = emailBody + (queryReport == null ? string.Empty : "<br />" + queryReport);
                    List<string> usersList = usersListID.Split(',').ToList();
                    foreach (var userID in usersList)
                    {
                        sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
                        Global.Global.sendEmail("School Transfer Report", emailBody, email);
                    }
                }
            }

        }

        public ActionResult ABCAnalysisReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ABCItemsDTO> abcReportItems = new List<ABCItemsDTO>();

            ABCItemsDTO emptyItem = new ABCItemsDTO();
            emptyItem.percentageRevenue = 0;
            emptyItem.percentageQuantity = 0;
            emptyItem.itemName = "";
            abcReportItems.Add(emptyItem);

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;

                if (fromDate == null || toDate == null)
                {
                    List<Item> items = db.Items.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).OrderByDescending(x => x.Price).ToList();

                    double itemsCount = 0;
                    int totalItemsCount = db.Items.Count();
                    decimal itemsPrice = 0;
                    decimal totalItemsPrice = Convert.ToDecimal(db.Items.Sum(x => x.Price));
                    foreach (var item in items)
                    {
                        itemsCount++;
                        itemsPrice += Convert.ToDecimal(item.Price);
                        ABCItemsDTO initiatingItem = new ABCItemsDTO();
                        initiatingItem.percentageRevenue = Math.Round((itemsPrice * 100) / totalItemsPrice);
                        initiatingItem.percentageQuantity = Math.Round((itemsCount * 100) / totalItemsCount);
                        initiatingItem.itemName = item.Name;

                        abcReportItems.Add(initiatingItem);
                    }

                    abcReportItems.Where(x => x.percentageQuantity >= 20).Select(x => x).ToList().ForEach(x => x.lineColor = "red");
                    abcReportItems.Where(x => x.percentageQuantity >= 50).Select(x => x).ToList().ForEach(x => x.lineColor = "yellow");
                }
                else
                {
                    List<Item> items = db.Items.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID) && x.ModifiedOn >= fromDate && x.ModifiedOn <= toDate).OrderByDescending(x => x.Price).ToList();

                    double itemsCount = 0;
                    int totalItemsCount = db.Items.Count();
                    decimal itemsPrice = 0;
                    decimal totalItemsPrice = Convert.ToDecimal(db.Items.Sum(x => x.Price));
                    foreach (var item in items)
                    {
                        itemsCount++;
                        itemsPrice += Convert.ToDecimal(item.Price);
                        ABCItemsDTO initiatingItem = new ABCItemsDTO();
                        initiatingItem.percentageRevenue = Math.Round((itemsPrice * 100) / totalItemsPrice);
                        initiatingItem.percentageQuantity = Math.Round((itemsCount * 100) / totalItemsCount);
                        initiatingItem.itemName = item.Name;

                        abcReportItems.Add(initiatingItem);
                    }

                    abcReportItems.Where(x => x.percentageQuantity >= 20).Select(x => x).ToList().ForEach(x => x.lineColor = "red");
                    abcReportItems.Where(x => x.percentageQuantity >= 50).Select(x => x).ToList().ForEach(x => x.lineColor = "yellow");
                }
            }

            return View(abcReportItems);
        }

        public ActionResult MaintenanceReport(Nullable<DateTime> fromDate = null, Nullable<DateTime> toDate = null)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                string suserID = User.Identity.GetUserId();
                Nullable<int> schoolID = db.AspNetUsers.FirstOrDefault(x => x.Id == suserID)?.SchoolID;

                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.Where(x => (schoolID == 0 ? true : x.SchoolID == schoolID)).ToList();
                List<Unit> units = db.Units.ToList();

                if(fromDate == null && toDate == null)
                {
                    itemsInStock = (from item in db.Items
                                    where item.SchoolID == schoolID
                                    group item by new { item.Name, item.AvailabilityStatusID, item.Price, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID, item.MaintenancePrice } into items
                                    select items).AsEnumerable().Select(
                                    items => new ItemsGroupedDTO()
                                    {
                                        GroupedId = items.FirstOrDefault().Id,
                                        Name = items.Key.Name,
                                        Name_Arabic = items.First().Name_Arabic,
                                        AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                        AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                        Quantity = items.Count(),
                                        MaintenancePrice = items.Key.MaintenancePrice,
                                        Price = items.Key.Price,
                                        ExpiryDate = items.Key.ExpiryDate,
                                        UnitID = items.Key.UnitID,
                                        UnitAmount = items.Key.UnitAmount,
                                        ItemStatusID = items.Key.ItemStatusID,
                                        Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                    }).ToList();
                }
                else
                {
                    itemsInStock = (from item in db.Items
                                    where item.SchoolID == schoolID
                                    where item.ModifiedOn >= fromDate && item.ModifiedOn <= toDate
                                    group item by new { item.Name, item.AvailabilityStatusID, item.Price, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID, item.MaintenancePrice } into items
                                    select items).AsEnumerable().Select(
                                    items => new ItemsGroupedDTO()
                                    {
                                        GroupedId = items.FirstOrDefault().Id,
                                        Name = items.Key.Name,
                                        Name_Arabic = items.First().Name_Arabic,
                                        AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                        AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                        Quantity = items.Count(),
                                        MaintenancePrice = items.Key.MaintenancePrice,
                                        Price = items.Key.Price,
                                        ExpiryDate = items.Key.ExpiryDate,
                                        UnitID = items.Key.UnitID,
                                        UnitAmount = items.Key.UnitAmount,
                                        ItemStatusID = items.Key.ItemStatusID,
                                        Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                    }).ToList();
                }
            }

            return View(itemsInStock);
        }

    }
}