using Inventory.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.WebApplication.Controllers
{
    public class PageManagementController : Controller
    {
        // GET: PageManagement
        public ActionResult Index()
        {
            List<PageManagement> pageManagement = new List<PageManagement>();

            using (var db = new InventoryEntities())
            {
                pageManagement = db.PageManagements.ToList();
            }
            
            return View(pageManagement);
        }
    }
}