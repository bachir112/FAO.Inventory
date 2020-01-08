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
    [Authorize(Roles = "Admin, SchoolManager, SchoolStockKeeper")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CategoriesPartial()
        {
            List<CategoryDTO> categoriesList = new List<CategoryDTO>();

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
                                    ItemsNames = itemsGroup.Select(x => x.Name).Distinct().ToList(),
                                    TotalQuantityInGroup = itemsGroup.Count(),
                                    Items = itemsGroup.ToList()
                                }).ToList();

                foreach(var category in categoriesList)
                {
                    category.ItemTypeInCategoryCount = itemsList.Where(x => x.GroupID == category.Id).Select(x => x.ItemsNames).SelectMany(x => x).Distinct().Count();
                    //var ss = itemsList.Where(x => x.GroupID == category.Id).Select(x => x.ItemsNames).SelectMany(x => x).ToList();
                    category.ItemInCategoryCount = itemsList.Where(x => x.GroupID == category.Id).Select(x => x).SelectMany(x => x.Items).Count();
                }
            }

            return View(categoriesList);
        }

        public ActionResult ItemsPartialDefault()
        {
            List<ItemsGroupedDTO> itemsInStock = new List<ItemsGroupedDTO>();

            using (var db = new InventoryEntities())
            {
                List<AvailabilityStatu> availabilityStatuses = db.AvailabilityStatus.ToList();
                List<Supplier> suppliers = db.Suppliers.ToList();

                itemsInStock = (from item in db.Items
                                             group item by new { item.Name, item.AvailabilityStatusID, item.ExpiryDate } into items
                                             select items).AsEnumerable().Select(
                                             items => new ItemsGroupedDTO()
                                             {
                                                 Name = items.Key.Name,
                                                 AvailabilityStatus = availabilityStatuses.FirstOrDefault(x => x.Id == items.Key.AvailabilityStatusID).Status,
                                                 AvailabilityStatusID = items.Key.AvailabilityStatusID,
                                                 Quantity = items.Count(),
                                                 LocationInStock = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.LocationInStock)).Select(x => x.LocationInStock)),
                                                 Description = string.Join(",", items.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description)),
                                                 ExpiryDate = items.Key.ExpiryDate
                                             }).ToList();
            }

            return View(itemsInStock);
        }


        public ActionResult Transactions()
        {
            return View();
        }

        public JsonResult AssignItems(int quantity, int AvailabilityStatusID, IEnumerable<Dictionary<string, object>> selectedItems)
        {
            List<string> result = new List<string>();

            var selectedItemsJSON = JsonConvert.SerializeObject(selectedItems);
            List<ItemsGroupedDTO> selectedItemsList = JsonConvert.DeserializeObject<List<ItemsGroupedDTO>>(selectedItemsJSON);

            using (var db = new InventoryEntities())
            {
                foreach(var item in selectedItemsList)
                {
                    var itemInDB = db.Items.Where(x => x.Name == item.Name && 
                                                    x.AvailabilityStatusID == item.AvailabilityStatusID && 
                                                    x.ExpiryDate == item.ExpiryDate && 
                                                    x.LocationInStock == item.LocationInStock && 
                                                    x.ReceivedOn == item.ReceivedOn && 
                                                    x.Description == item.Description 
                                                    ).Select(x => x).Take(quantity).ToList();
                    itemInDB.ForEach(x => x.AvailabilityStatusID = AvailabilityStatusID);

                    Transaction newTransaction = new Transaction();
                    newTransaction.ItemName = item.Name;
                    newTransaction.OldAvailabilityStatus = 1;
                    newTransaction.NewAvailabilityStatus = item.AvailabilityStatusID;
                    newTransaction.StockKeeper = User.Identity.GetUserId();
                    newTransaction.Quantity = quantity;

                    db.Transactions.Add(newTransaction);

                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemsInCategory(Nullable<int> categoryID = null)
        {
            List<string> result = new List<string>();

            using (var db = new InventoryEntities())
            {
                result = db.Items.Where(x => (categoryID == null) ? true : x.CategoryID == categoryID).Select(x => x.Name).ToList();
                List<string> searchableItems = db.ItemsSearchValues.Where(x => (categoryID == null) ? true : x.CategoryID == categoryID).Select(x => x.ItemName).ToList();
                result.AddRange(searchableItems);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddItemToSearchble(string itemName, int categoryID)
        {
            string result = "error";

            using (var db = new InventoryEntities())
            {
                ItemsSearchValue newItem = new ItemsSearchValue();
                newItem.ItemName = itemName;
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