using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inventory.DataObjects.EDM;
using Microsoft.AspNet.Identity;

namespace Inventory.WebApplication.Controllers
{
    public class AvailabilityStatusController : Controller
    {
        private InventoryEntities db = new InventoryEntities();

        // GET: AvailabilityStatus
        public ActionResult Index()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Users"))
            {
                List<Item> items = db.Items.ToList();
                List<AvailabilityStatu> availabilityStatus = db.AvailabilityStatus.ToList();
                availabilityStatus.ForEach(x => x.CanDelete = (items.Where(y => y.AvailabilityStatusID == x.Id).Select(y => y).Count() > 0 ? false : true));
                return View(availabilityStatus);
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // GET: AvailabilityStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailabilityStatu availabilityStatu = db.AvailabilityStatus.Find(id);
            if (availabilityStatu == null)
            {
                return HttpNotFound();
            }
            return View(availabilityStatu);
        }

        // GET: AvailabilityStatus/Create
        public ActionResult Create()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Reports"))
            {
                int highestIndex = db.AvailabilityStatus.Max(x => x.Id);
                ViewBag.NewID = highestIndex + 1;
                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // POST: AvailabilityStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Status,Description,Status_Arabic")] AvailabilityStatu availabilityStatu)
        {
            if (ModelState.IsValid)
            {
                db.AvailabilityStatus.Add(availabilityStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(availabilityStatu);
        }

        // GET: AvailabilityStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Reports"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AvailabilityStatu availabilityStatu = db.AvailabilityStatus.Find(id);
                if (availabilityStatu == null)
                {
                    return HttpNotFound();
                }
                return View(availabilityStatu);
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // POST: AvailabilityStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,Status,Description,Status_Arabic")] AvailabilityStatu availabilityStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(availabilityStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(availabilityStatu);
        }

        // GET: AvailabilityStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Reports"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AvailabilityStatu availabilityStatu = db.AvailabilityStatus.Find(id);
                if (availabilityStatu == null)
                {
                    return HttpNotFound();
                }
                return View(availabilityStatu);
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // POST: AvailabilityStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            AvailabilityStatu availabilityStatu = db.AvailabilityStatus.Find(id);
            db.AvailabilityStatus.Remove(availabilityStatu);
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
