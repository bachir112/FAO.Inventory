using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inventory.DataObjects.DTO;
using Inventory.DataObjects.EDM;
using Microsoft.AspNet.Identity;

namespace Inventory.WebApplication.Controllers
{
    [Authorize(Roles = "Admin, SchoolManager, SchoolStockKeeper")]
    public class ItemsController : Controller
    {
        private InventoryEntities db = new InventoryEntities(Global.Global.GetSchoolCookieValue());

        public ActionResult Route()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            return View();
        }

        public ActionResult Pricing()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Pricing"))
            {
                ViewBag.Categories = db.Categories
                        .Select(x => new CategoryDTO
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Name_Arabic = x.Name_Arabic,
                            Picture = x.Picture,
                            ParentCategory = x.ParentCategory
                        }).ToList();

                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // GET: Items
        public ActionResult Index()
        {
            return View(db.Items.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        public JsonResult EditPrice(decimal itemPrice, string listOfIDs)
        {
            string result = "Error";

            try
            {
                List<string> listOfItemsNames = new List<string>();
                List<int> listOfItemsIDs = listOfIDs.Split(',').Select(Int32.Parse).ToList();
                foreach (var ID in listOfItemsIDs)
                {
                    Item item = db.Items.FirstOrDefault(x => x.Id == ID);
                    if(item != null)
                    {
                        listOfItemsNames.Add(item.Name + "(" + item.Name_Arabic + ")");
                        item.Price = itemPrice;
                    }
                    db.SaveChanges();

                    result = "Success";
                }

                listOfItemsNames = listOfItemsNames.Distinct().Select(x => x).ToList();
                var itemsNames = string.Join(",", listOfItemsNames);
                Logging logging = new Logging();
                logging.UserID = User.Identity.GetUserId();
                logging.Action = "User " + User.Identity.Name + " changed the price of " + listOfItemsIDs.Count().ToString() + " " + itemsNames + " to LL" + itemPrice.ToString() + " on " + DateTime.Now.ToString();
                db.Loggings.Add(logging);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                result = "Error";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Items/Create
        public ActionResult Create(Nullable<bool> newItem = null)
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "AddItems"))
            {
                List<CategoryDTO> categoriesList = db.Categories
                                    .Select(x => new CategoryDTO
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Name_Arabic = x.Name_Arabic,
                                        Picture = x.Picture,
                                        ParentCategory = x.ParentCategory
                                    }).ToList();

                List<Supplier> suppliersList = db.Suppliers.ToList();
                List<AvailabilityStatu> availabilityStatusList = db.AvailabilityStatus.ToList();
                List<ItemStatu> itemStatusList = db.ItemStatus.ToList();
                List<Unit> unitList = db.Units.ToList();

                ViewBag.LocationInStock = db.Items.Where(x => x.LocationInStock != null && x.LocationInStock.Trim() != string.Empty)
                                                  .Select(x => x.LocationInStock)
                                                  .Distinct()
                                                  .ToList();

                ViewBag.ItemDescription = db.Items.Where(x => x.Description != null && x.Description.Trim() != string.Empty)
                                                  .Select(x => x.Description)
                                                  .Distinct()
                                                  .ToList();

                ViewBag.CategoriesList = categoriesList;
                ViewBag.SuppliersList = suppliersList;
                ViewBag.AvailabilityStatusList = availabilityStatusList;
                ViewBag.ItemStatusList = itemStatusList;
                ViewBag.UnitList = unitList;
                ViewBag.NewItem = newItem;

                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Name_Arabic,AvailabilityStatusID,ItemStatusID,LocationInStock,UnitID,UnitAmount,ExpiryDate,Description,SupplierID,CategoryID,Quantity")] Item item)
        {
            if (ModelState.IsValid)
            {
                int quantity = item.Quantity == null ? Convert.ToInt32(item.UnitAmount): Convert.ToInt32(item.Quantity);
                item.UnitAmount = item.UnitID == 1 ? null : item.UnitAmount;

                Nullable<bool> expandable = db.ItemsSearchValues.FirstOrDefault(x => x.ItemName == item.Name && x.ItemName_Arabic == item.Name_Arabic)?.Expandable;

                for (int i=0; i < quantity; i++)
                {
                    Item newItem = new Item();
                    newItem.Name = item.Name;
                    newItem.Name_Arabic = item.Name_Arabic;
                    newItem.AvailabilityStatusID = item.AvailabilityStatusID;
                    newItem.CategoryID = item.CategoryID;
                    newItem.Description = item.Description == null ? string.Empty : item.Description;
                    newItem.ExpiryDate = item.ExpiryDate;
                    newItem.ItemStatusID = item.ItemStatusID;
                    newItem.LocationInStock = item.LocationInStock;
                    newItem.Price = item.Price;
                    newItem.SupplierID = item.SupplierID;
                    newItem.UnitAmount = item.UnitAmount;
                    newItem.UnitID = item.UnitID;
                    newItem.Expandable = expandable;
                    newItem.ReceivedOn = DateTime.Now;
                    newItem.ModifiedBy = User.Identity.GetUserId();

                    db.Items.Add(newItem);
                    db.SaveChanges();
                }

                Logging logging = new Logging();
                logging.UserID = User.Identity.GetUserId();
                logging.Action = "User " + User.Identity.Name + " created " + quantity + " " + item.Name + "(" + item.Name_Arabic + ") on " + item.ReceivedOn;
                db.Loggings.Add(logging);
                db.SaveChanges();

                return RedirectToAction("Create", new { newItem = true });
            }

            return RedirectToAction("Create", new { newItem = false });
        }

        // GET: Items/Edit/5
        public ActionResult EditSection()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "DeleteItems"))
            {
                List<CategoryDTO> categoriesList = db.Categories
                                    .Select(x => new CategoryDTO
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Picture = x.Picture,
                                        ParentCategory = x.ParentCategory
                                    }).ToList();

                List<Supplier> suppliersList = db.Suppliers.ToList();
                List<AvailabilityStatu> availabilityStatusList = db.AvailabilityStatus.ToList();
                List<ItemStatu> itemStatusList = db.ItemStatus.ToList();
                List<Unit> unitList = db.Units.ToList();

                ViewBag.LocationInStock = db.Items.Where(x => x.LocationInStock != null && x.LocationInStock.Trim() != string.Empty)
                                                  .Select(x => x.LocationInStock)
                                                  .Distinct()
                                                  .ToList();

                ViewBag.ItemDescription = db.Items.Where(x => x.Description != null && x.Description.Trim() != string.Empty)
                                                  .Select(x => x.Description)
                                                  .Distinct()
                                                  .ToList();

                ViewBag.CategoriesList = categoriesList;
                ViewBag.SuppliersList = suppliersList;
                ViewBag.AvailabilityStatusList = availabilityStatusList;
                ViewBag.ItemStatusList = itemStatusList;
                ViewBag.UnitList = unitList;

                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Name_Arabic,AvailabilityStatusID,ItemStatusID,LocationInStock,UnitID,UnitAmount,ExpiryDate,Description,SupplierID,CategoryID")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public JsonResult DeleteItems(string listOfIDs, int quantityToDelete)
        {
            string result = "Error";

            try
            {
                List<string> listOfItemsNames = new List<string>();
                List<int> listOfItemsIDs = listOfIDs.Split(',').Select(Int32.Parse).ToList().Take(quantityToDelete).ToList();
                foreach (var ID in listOfItemsIDs)
                {
                    Item item = db.Items.FirstOrDefault(x => x.Id == ID);
                    listOfItemsNames.Add(item.Name + "(" + item.Name_Arabic + ")");
                    db.Items.Remove(item);
                    db.SaveChanges();
                }

                listOfItemsNames = listOfItemsNames.Distinct().Select(x => x).ToList();
                var itemsNames = string.Join(",", listOfItemsNames);
                Logging logging = new Logging();
                logging.UserID = User.Identity.GetUserId();
                logging.Action = "User " + User.Identity.Name + " deleted " + quantityToDelete + " " + itemsNames + " on " + DateTime.Now.ToString();
                db.Loggings.Add(logging);
                db.SaveChanges();

                result = "Success";
            }
            catch (Exception ex)
            {
                result = "Error";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
