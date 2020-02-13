﻿using Inventory.DataObjects.DTO;
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
    [Authorize(Roles = "Admin, SchoolManager, SchoolStockKeeper")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Reports()
        {
            return View();
        }


        public ActionResult CategoriesPartial(string query)
        {
            List<CategoryDTO> categoriesList = new List<CategoryDTO>();

            int queryID = (query == "IN" ? 1 : query == "OUT" ? 2 : query == "TRASH" ? 3 : -1);

            using (var db = new InventoryEntities())
            {
                categoriesList = db.Categories
                                    .Select(x => new CategoryDTO
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Picture = x.Picture,
                                        ParentCategory = x.ParentCategory
                                    }).ToList();

                var itemsList = (from items in db.Items
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
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.CategoryID == (categoryID == null ? item.CategoryID : categoryID)
                                && item.AvailabilityStatusID == (queryID == -1 ? item.AvailabilityStatusID : queryID)
                                group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate, item.UnitID, item.UnitAmount, item.ItemStatusID, item.Description } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    ItemsIDs = string.Join(",", items.Select(x => x.Id).ToList()),
                                    Name = items.Key.Name,
                                    AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                    AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                    Quantity = items.Count(),
                                    LocationInStock = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock + " (" + items.Where(y => y.LocationInStock == x.LocationInStock).Select(y => y).Count().ToString() + ")").Distinct()),
                                    Description = string.Join(", ", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description + " (" + items.Where(y => y.Description == x.Description).Select(y => y).Count().ToString() + ")").Distinct()),
                                    //Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ItemStatusID = items.Key.ItemStatusID,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID).Name
                                }).ToList();

                ViewBag.CategoryName = categoryID == null ? null : db.Categories.First(x => x.Id == categoryID).Name;
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
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();
                List<Unit> units = db.Units.ToList();

                itemsInStock = (from item in db.Items
                                where item.CategoryID == (categoryID == null ? item.CategoryID : categoryID)
                                && (fromDate == null ? true : item.ReceivedOn >= fromDate)
                                && (toDate == null ? true : item.ReceivedOn <= toDate)
                                group item by new { item.Name, item.ExpiryDate, item.UnitID, item.UnitAmount, item.SupplierID, item.ReceivedOn, item.Price } into items
                                select items).AsEnumerable().Select(
                                items => new ItemsGroupedDTO()
                                {
                                    ItemsIDs = string.Join(",", items.Select(x => x.Id).ToList()),
                                    Name = items.Key.Name,
                                    Price = items.Key.Price,
                                    Supplier = suppliers.FirstOrDefault(x => x.Id == items.Key.SupplierID)?.Supplier1,
                                    Quantity = items.Count(),
                                    ExpiryDate = items.Key.ExpiryDate,
                                    UnitID = items.Key.UnitID,
                                    UnitAmount = items.Key.UnitAmount,
                                    ReceivedOn = items.Key.ReceivedOn,
                                    Unit = units.FirstOrDefault(x => x.Id == items.Key.UnitID)?.Name
                                }).ToList();
            }
            
            return View(itemsInStock);
        }

        public ActionResult Transactions()
        {
            return View();
        }

        public ActionResult TransactionsIntoStock()
        {
            using (var db = new InventoryEntities())
            {
                ViewBag.ToWhom = db.Transactions.Where(x => x.ToWhom != null && x.ToWhom.Trim() != string.Empty)
                                                .Select(x => x.ToWhom)
                                                .Distinct()
                                                .ToList();

                ViewBag.Description = db.Transactions.Where(x => x.Description != null && x.Description.Trim() != string.Empty)
                                                .Select(x => x.Description)
                                                .Distinct()
                                                .ToList();
            }

            return View();
        }

        public ActionResult TransactionsOutOfStock()
        {
            using (var db = new InventoryEntities())
            {
                ViewBag.ToWhom = db.Transactions.Where(x => x.ToWhom != null && x.ToWhom.Trim() != string.Empty)
                                                .Select(x => x.ToWhom)
                                                .Distinct()
                                                .ToList();

                ViewBag.Description = db.Transactions.Where(x => x.Description != null && x.Description.Trim() != string.Empty)
                                                .Select(x => x.Description)
                                                .Distinct()
                                                .ToList();
            }

            return View();
        }

        public ActionResult Deteriorated()
        {
            return View();
        }

        public ActionResult RecentTransactionsPartial()
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
                    Unit = db.Units.FirstOrDefault(y => y.Id == x.UnitID) != null ? db.Units.FirstOrDefault(y => y.Id == x.UnitID).Name : string.Empty,
                    UnitAmount = x.UnitAmount,
                    NewAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.NewAvailabilityStatus).Status : string.Empty,
                    OldAvailabilityStatus = db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus) != null ? db.AvailabilityStatus.FirstOrDefault(y => y.Id == x.OldAvailabilityStatus).Status : string.Empty,
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
                foreach(var item in selectedItemsList)
                {
                    string removeSection = item.Description
                                           .Substring(item.Description.LastIndexOf("("), 
                                                      item.Description.LastIndexOf(")") - item.Description.LastIndexOf("(") + 1
                                                     );

                    item.Description = item.Description.Replace(removeSection, string.Empty);

                    var itemInDB = db.Items.Where(x => x.Name == item.Name && 
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

                    itemInDB = itemInDB.Take(quantity).ToList();

                    itemInDB.ForEach(x => x.AvailabilityStatusID = AvailabilityStatusID);
                    itemInDB.ForEach(x => x.Description = Description);

                    Transaction newTransaction = new Transaction();
                    newTransaction.ItemName = item.Name;
                    newTransaction.OldAvailabilityStatus = item.AvailabilityStatusID;
                    newTransaction.NewAvailabilityStatus = AvailabilityStatusID;
                    newTransaction.StockKeeper = User.Identity.GetUserId();
                    newTransaction.Quantity = quantity;
                    newTransaction.ToWhom = ToWhom;
                    newTransaction.Description = Description;
                    newTransaction.UnitID = item.UnitID;
                    newTransaction.UnitAmount = item.UnitAmount;
                    newTransaction.TransactionDate = DateTime.Now;

                    db.Transactions.Add(newTransaction);

                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemsInCategory(Nullable<int> categoryID = null)
        {
            //List<string> result = new List<string>();
            List<SearchItemsDTO> result = new List<SearchItemsDTO>();

            using (var db = new InventoryEntities())
            {
                //result = db.Items
                //           .Where(x => (categoryID == null) ? true : x.CategoryID == categoryID)
                //           .Select(x => x.Name)
                //           .Distinct()
                //           .ToList();

                //List<string> searchableItems = db.ItemsSearchValues
                //                                 .Where(x => (categoryID == null) ? true : x.CategoryID == categoryID)
                //                                 .Select(x => x.ItemName)
                //                                 .Distinct()
                //                                 .ToList();
                //result.AddRange(searchableItems);                

                List<SearchItemsDTO>  items = db.Items
                                                .Where(x => (categoryID == null) ? true : x.CategoryID == categoryID)
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

            using (var db = new InventoryEntities())
            {
                ItemsSearchValue newItem = new ItemsSearchValue();
                newItem.ItemName = itemName;
                newItem.ItemName_Arabic = itemName_Arabic;
                newItem.CategoryID = categoryID;

                db.ItemsSearchValues.Add(newItem);
                db.SaveChanges();
                result = "success";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}