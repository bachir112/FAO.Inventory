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
        private InventoryEntities db = new InventoryEntities();

        public ActionResult Route()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            ViewBag.Categories = db.Categories
                    .Select(x => new CategoryDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Picture = x.Picture,
                        ParentCategory = x.ParentCategory
                    }).ToList();

            return View();
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
                List<int> listOfItemsIDs = listOfIDs.Split(',').Select(Int32.Parse).ToList();
                foreach (var ID in listOfItemsIDs)
                {
                    Item item = db.Items.FirstOrDefault(x => x.Id == ID);
                    if(item != null)
                    {
                        item.Price = itemPrice;
                    }
                    db.SaveChanges();

                    result = "Success";
                }
            }
            catch(Exception ex)
            {
                result = "Error";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Items/Create
        public ActionResult Create()
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

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Name_Arabic,AvailabilityStatusID,ItemStatusID,LocationInStock,UnitID,UnitAmount,ExpiryDate,Description,SupplierID,CategoryID,Quantity")] Item item)
        {
            if (ModelState.IsValid)
            {
                List<Item> addedItems = new List<Item>();

                int quantity = item.Quantity == null ? Convert.ToInt32(item.UnitAmount): Convert.ToInt32(item.Quantity);
                item.UnitAmount = item.UnitID == 1 ? null : item.UnitAmount;
                int maxID = db.Items.Max(x => x.Id);
                for (int i=0; i < quantity; i++)
                {
                    Item newItem = new Item();
                    newItem.Name = item.Name;
                    newItem.Name_Arabic = item.Name_Arabic;
                    newItem.AvailabilityStatusID = item.AvailabilityStatusID;
                    newItem.CategoryID = item.CategoryID;
                    newItem.Description = item.Description;
                    newItem.ExpiryDate = item.ExpiryDate;
                    newItem.ItemStatusID = item.ItemStatusID;
                    newItem.LocationInStock = item.LocationInStock;
                    newItem.Price = item.Price;
                    newItem.SupplierID = item.SupplierID;
                    newItem.UnitAmount = item.UnitAmount;
                    newItem.UnitID = item.UnitID;
                    newItem.ReceivedOn = DateTime.Now;
                    newItem.ModifiedBy = User.Identity.GetUserId();

                    item.Id = maxID + i;
                    addedItems.Add(newItem);
                }

                db.Items.AddRange(addedItems);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return RedirectToAction("Create");
        }

        // GET: Items/Edit/5
        public ActionResult EditSection()
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

        // GET: Items/Edit/5
        public ActionResult Edit(int id)
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

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
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
