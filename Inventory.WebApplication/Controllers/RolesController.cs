using Inventory.DataObjects.EDM;
using Inventory.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory.WebApplication.Controllers
{
    public class RolesController : Controller
    {
        private InventoryEntities db = new InventoryEntities(Global.Global.GetSchoolCookieValue());

        // GET: Roles
        public ActionResult Index()
        {
            ViewBag.PageManagement = Global.Global.AllowedPages(User.Identity.GetUserId());
            if (Global.Global.isAllowed(User.Identity.GetUserId(), "Roles"))
            {
                List<AspNetRole> roles = db.AspNetRoles.ToList();
                return View(roles);
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }
    }
}