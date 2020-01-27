using Inventory.DataObjects.DTO;
using Inventory.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.WebApplication.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InventoryGeneralReport()
        {
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

        public ActionResult ItemsInReport()
        {
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

        public ActionResult SearchForNonConsumableReport()
        {
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

        public ActionResult DailyReport()
        {
            List<TransactionsReminder> transactionsReminders = new List<TransactionsReminder>();

            using (var db = new InventoryEntities())
            {
                transactionsReminders.AddRange(db.TransactionsReminders.ToList());
            }

            return View(transactionsReminders);
        }

        public ActionResult ConsumableItemsReport()
        {
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.Consumable == true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
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

        public ActionResult NonConsumableItemsReport()
        {
            List<ItemsInReportQuery> itemsInReportQuery = new List<ItemsInReportQuery>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Unit> units = db.Units.ToList();


                List<ItemsGroupedDTO> itemsInStock = (from item in db.Items
                                                      where (item.Consumable != true) && (item.AvailabilityStatusID == 1 || item.AvailabilityStatusID == 2)
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

        public ActionResult FullInventoryGeneralReport()
        {
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

        public ActionResult BudgetLineStatementOfAccountReport()
        {
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

        public ActionResult QuantityReport()
        {
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

        public ActionResult SchoolTransferReport()
        {

            return View();
        }

        public ActionResult ABCAnalysisReport()
        {

            return View();
        }

    }
}