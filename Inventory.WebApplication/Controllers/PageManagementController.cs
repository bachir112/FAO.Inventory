﻿using Inventory.DataObjects.EDM;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.WebApplication.Controllers
{
    [Authorize]
    public class PageManagementController : Controller
    {
        // GET: PageManagement
        public ActionResult Index()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "PageManagement"))
            {
                List<PageManagement> pageManagement = new List<PageManagement>();

                using (var db = new InventoryEntities(Global.Global.GetSchoolCookieValue()))
                {
                    pageManagement = db.PageManagements.OrderBy(x => x.RoleName).ToList();

                    ViewBag.AspNetRoles = db.AspNetRoles.ToList();
                }

                return View(pageManagement);
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }


        public JsonResult AssignPageManagement(string pageName, string userRole, bool allowed)
        {
            List<string> result = new List<string>();

            using (var db = new InventoryEntities(Global.Global.GetSchoolCookieValue()))
            {
                List<PageManagement> pageManagements = db.PageManagements.Where(x => x.PageName == pageName && x.RoleName == userRole).ToList();

                if(pageManagements.Count() == 0)
                {
                    PageManagement newPageManagement = new PageManagement();
                    newPageManagement.PageName = pageName;
                    newPageManagement.RoleName = userRole;
                    newPageManagement.Allowed = allowed;

                    db.PageManagements.Add(newPageManagement);
                    db.SaveChanges();
                }
                else
                {
                    PageManagement pageManagement = pageManagements.FirstOrDefault();
                    pageManagement.Allowed = allowed;

                    db.SaveChanges();
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}