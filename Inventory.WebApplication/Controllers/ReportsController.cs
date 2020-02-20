using Inventory.DataObjects.DTO;
using Inventory.DataObjects.EDM;
using Inventory.WebApplication.Models;
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
                    ViewBag.ItemsNames = db.Items.Select(x => new ItemNames
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
                ReportSetting reportSettings = db.ReportSettings.FirstOrDefault(x => x.ReportID == ReportID);

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
                ReportQuery newReportQuery = new ReportQuery();
                newReportQuery.ReportID = reportID;
                newReportQuery.ItemName = itemName;
                newReportQuery.AvailabilityStatusID = itemAvailability;
                newReportQuery.MinimumPrice = minPrice;
                newReportQuery.MaximumPrice = maxPrice;
                newReportQuery.MinimumQuantity = minQuantity;
                newReportQuery.MaximumQuantity = maxQuantity;

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
                    ReportQuery query = db.ReportQueries.First(x => x.Id == queryID);
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
                reportSettings = db.ReportSettings.FirstOrDefault(x => x.ReportID == reportID);
                listOfReportQuery = db.ReportQueries.Where(x => x.ReportID == reportID).ToList();

                reportSettingsDict.Add("reportSettings", reportSettings);
                reportSettingsDict.Add("listOfReportQuery", listOfReportQuery);
            }

            return Json(reportSettingsDict, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InventoryGeneralReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
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
            }

            return View(itemsInStock);
        }
        public void InventoryGeneralReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
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

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInStock);

            string emailBody = "Please find attached to this email a copy of the Inventory General Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult ItemsInReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.AvailabilityStatusID == 1
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Description } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
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

        public void ItemsInReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.AvailabilityStatusID == 1
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Description } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Description = items.Key.Description,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInStock);

            string emailBody = "Please find attached to this email a copy of the Items In Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult SearchForNonConsumableReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<TransactionDTO> result = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                List<Transaction> transactions = db.Transactions.ToList();
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
            }

            return View(result);
        }

        public void SearchForNonConsumableReport_Email()
        {
            string sendToEmail = string.Empty;
            List<TransactionDTO> result = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                List<Transaction> transactions = db.Transactions.ToList();
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

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(result);

            string emailBody = "Please find attached to this email a copy of the Search For Non Consumable Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult DailyReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<TransactionsReminder> transactionsReminders = new List<TransactionsReminder>();

            using (var db = new InventoryEntities())
            {
                transactionsReminders.AddRange(db.TransactionsReminders.ToList());
            }

            return View(transactionsReminders);
        }

        public void DailyReport_Email()
        {
            string sendToEmail = string.Empty;
            List<TransactionsReminder> transactionsReminders = new List<TransactionsReminder>();

            using (var db = new InventoryEntities())
            {
                transactionsReminders.AddRange(db.TransactionsReminders.ToList());

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(transactionsReminders);

            string emailBody = "Please find attached to this email a copy of the Daily Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult ConsumableItemsReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.Expandable == true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
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

        public void ConsumableItemsReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.Expandable == true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
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

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInReportQuery);

            string emailBody = "Please find attached to this email a copy of the Consumable Items Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult NonConsumableItemsReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.Expandable != true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
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

        public void NonConsumableItemsReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.Expandable != true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
                                                      group item by new { item.Name, item.UnitID, item.UnitAmount } into items
                                                      select items).AsEnumerable().Select(
                                                      items => new ItemsGroupedDTO()
                                                      {
                                                          GroupedId = items.FirstOrDefault().Id,
                                                          Name = items.Key.Name,
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

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInReportQuery);

            string emailBody = "Please find attached to this email a copy of the Non-Consumable Items Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult FullInventoryGeneralReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
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
            }

            return View(itemsInStock);
        }

        public void FullInventoryGeneralReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
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

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInStock);

            string emailBody = "Please find attached to this email a copy of the Full Inventory General Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }


        public ActionResult BudgetLineStatementOfAccountReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
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

        public void BudgetLineStatementOfAccountReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.AvailabilityStatusID, item.UnitID, item.UnitAmount, item.Price, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name,
                                    Price = items.Key.Price,
                                    TotalPrice = items.Key.Price * items.Count()
                                }).ToList();

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInStock);

            string emailBody = "Please find attached to this email a copy of the BudgetLine Statement Of Account Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }


        public ActionResult QuantityReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.UnitID, item.UnitAmount, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Quantity = items.Count(),
                                    QuantityIn = items.Where(x => x.AvailabilityStatusID == 1 || x.AvailabilityStatusID ==2).Count(),
                                    QuantityOut = items.Where(x => x.AvailabilityStatusID == 3).Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    CategoryID = items.Key.CategoryID,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();
            }

            return View(itemsInStock);
        }

        public void QuantityReport_Email()
        {
            string sendToEmail = string.Empty;
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = (from item in db.Items
                                group item by new { item.Name, item.UnitID, item.UnitAmount, item.CategoryID } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    GroupedId = items.FirstOrDefault().Id,
                                    Name = items.Key.Name,
                                    Quantity = items.Count(),
                                    QuantityIn = items.Where(x => x.AvailabilityStatusID == 1 || x.AvailabilityStatusID == 2).Count(),
                                    QuantityOut = items.Where(x => x.AvailabilityStatusID == 3).Count(),
                                    Category = categories.FirstOrDefault(x => x.Id == items.Key.CategoryID)?.Name,
                                    CategoryID = items.Key.CategoryID,
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInStock);

            string emailBody = "Please find attached to this email a copy of the Quantity Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }


        public ActionResult SchoolTransferReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<TransactionDTO> itemsInStock = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = db.Transactions.Where(x => x.NewAvailabilityStatus == 1002).Select(
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
            }

            return View(itemsInStock);
        }

        public void SchoolTransferReport_Email()
        {
            string sendToEmail = string.Empty;
            List<TransactionDTO> itemsInStock = new List<TransactionDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();
                List<Category> categories = db.Categories.ToList();

                itemsInStock = db.Transactions.Where(x => x.NewAvailabilityStatus == 1002).Select(
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

                string userID = User.Identity.GetUserId();
                sendToEmail = db.AspNetUsers.First(x => x.Id == userID).Email;
            }

            Global.Global.ExportDataSetToExcel(itemsInStock);

            string emailBody = "Please find attached to this email a copy of the School Transfer Report";
            Global.Global.sendEmail("Inventory General Report", emailBody, sendToEmail);
        }

        public ActionResult ABCAnalysisReport()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            List<ABCItemsDTO> abcReportItems = new List<ABCItemsDTO>();

            ABCItemsDTO emptyItem = new ABCItemsDTO();
            emptyItem.percentageRevenue = 0;
            emptyItem.percentageQuantity = 0;
            emptyItem.itemName = "";
            abcReportItems.Add(emptyItem);

            using (var db = new InventoryEntities())
            {
                List<Item> items = db.Items.OrderByDescending(x => x.Price).ToList();

                double itemsCount = 0;
                int totalItemsCount = db.Items.Count();
                decimal itemsPrice = 0;
                decimal totalItemsPrice = Convert.ToDecimal(db.Items.Sum(x => x.Price));
                foreach (var item in items)
                {
                    itemsCount++;
                    itemsPrice += Convert.ToDecimal(item.Price);
                    ABCItemsDTO initiatingItem = new ABCItemsDTO();
                    initiatingItem.percentageRevenue = Math.Round((itemsPrice * 100)/ totalItemsPrice);
                    initiatingItem.percentageQuantity = Math.Round((itemsCount * 100) / totalItemsCount);
                    initiatingItem.itemName = item.Name;

                    abcReportItems.Add(initiatingItem);
                }

                abcReportItems.Where(x => x.percentageQuantity >= 20).Select(x => x).ToList().ForEach(x => x.lineColor = "red");
                abcReportItems.Where(x => x.percentageQuantity >= 50).Select(x => x).ToList().ForEach(x => x.lineColor = "yellow");

                //ABCItemsDTO initiatingItem = new ABCItemsDTO();
                //initiatingItem.itemName = "";
                //initiatingItem.percentageQuantity = 0;
                //initiatingItem.percentageRevenue = 0;
                //initiatingItem.lineColor = "red";

                //abcReportItems.Add(initiatingItem);

                //List<ABCItemsDTO> abcReportItemsList = (from item in db.Items
                //                                         group item by new { item.Name } into itemsList
                //                                         select new ABCItemsDTO()
                //                                         {
                //                                             itemName = itemsList.Key.Name,
                //                                             percentageQuantity = itemsList.Count(),
                //                                             percentageRevenue = ((itemsList.Sum(x => x.Price) / totalAmount) * 100) == null ? 0 : ((itemsList.Sum(x => x.Price) / totalAmount) * 100),
                //                                             lineColor = "red"
                //                                         }).OrderBy(x => x.percentageQuantity).ToList();

                //abcReportItems.AddRange(abcReportItemsList);
            }

            return View(abcReportItems);
        }

    }
}