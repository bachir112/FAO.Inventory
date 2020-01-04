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
        public ActionResult Create([Bind(Include = "Id,Name,AvailabilityStatusID,ItemStatusID,LocationInStock,UnitID,UnitAmount,ExpiryDate,Description,SupplierID,CategoryID,Quantity")] Item item)
        {
            if (ModelState.IsValid)
            {
                int quantity = item.Quantity == 0 ? item.UnitAmount : item.Quantity;
                for (int i=0; i < item.Quantity; i++)
                {
                    db.Items.Add(item);
                }
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return RedirectToAction("Create");
        }

        // GET: Items/Edit/5
        public ActionResult EditSection()
        {
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
        public ActionResult Edit([Bind(Include = "Id,Name,AvailabilityStatusID,ItemStatusID,LocationInStock,UnitID,UnitAmount,ExpiryDate,Description,SupplierID,CategoryID")] Item item)
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
