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
        private InventoryEntities db = new InventoryEntities();

        // GET: Roles
        public async Task<ActionResult> Index()
        {
            List<AspNetUser> listAspNetUser = db.AspNetUsers.ToList();

            using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                foreach (var user in listAspNetUser)
                {
                    var userRole = await userManager.GetRolesAsync(user.Id);
                    user.UserRole = userRole.FirstOrDefault();
                }
            }

            return View(listAspNetUser);
        }
    }
}