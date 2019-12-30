using Inventory.DataObjects.DTO;
using Inventory.DataObjects.EDM;
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
                    category.ItemTypeInCategoryCount = itemsList.Where(x => x.GroupID == category.Id).Select(x => x.ItemsNames).Distinct().Count();
                    category.ItemInCategoryCount = itemsList.Where(x => x.GroupID == category.Id).Select(x => x).Count();
                }
            }

            return View(categoriesList);
        }

        public ActionResult Transactions()
        {
            return View();
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