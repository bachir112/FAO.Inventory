using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Inventory.DataObjects.EDM;
using Inventory.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Inventory.WebApplication.Global;

namespace Inventory.WebApplication.Controllers
{
    [Authorize(Roles = "Admin, SchoolManager, SchoolStockKeeper")]
    public class UsersController : Controller
    {
        private InventoryEntities db = new InventoryEntities();

        // GET: AspNetUsers
        public async Task<ActionResult> Index()
        {
            List<AspNetUser> listAspNetUser = db.AspNetUsers.ToList();

            using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                foreach(var user in listAspNetUser)
                {
                    var userRole = await userManager.GetRolesAsync(user.Id);
                    user.UserRole = userRole.FirstOrDefault();
                }
            }

            return View(listAspNetUser);
        }

        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FullName")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUser);
        }

        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var user = userManager.FindById(id);
                string rolename = userManager.GetRoles(user.Id).FirstOrDefault();

                ViewBag.UserRole = rolename;
            }

            ViewBag.AspNetRoles = db.AspNetRoles.ToList();

            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,UserRole")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ApplicationDbContext())
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    var roles = await userManager.GetRolesAsync(aspNetUser.Id);
                    await userManager.RemoveFromRolesAsync(aspNetUser.Id, roles.ToArray());

                    userManager.AddToRole(aspNetUser.Id, aspNetUser.UserRole);
                }

                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> ResetPassword(string newPassword, string userID = null)
        {
            string response = Global.Global.EnumsError;

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                
                userID = userID == null ? User.Identity.GetUserId() : userID;
                string hashedNewPassword = userManager.PasswordHasher.HashPassword(newPassword);

                ApplicationUser cUser = await userStore.FindByIdAsync(userID);
                await userStore.SetPasswordHashAsync(cUser, hashedNewPassword);
                await userStore.UpdateAsync(cUser);


                ApplicationUser user = userManager.FindByName(User.Identity.Name);
                string mail = user.Email;

                bool emailSent = Global.Global.sendEmail(
                            User.Identity.Name + " your password has been reset",

                            "Your new password is: " + newPassword,

                            mail
                          );

                if (emailSent)
                {
                    response = Global.Global.EnumsSuccess;
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AddRole(string roleName)
        {
            string response = Global.Global.EnumsError;

            try
            {
                AspNetRole aspNetRole = new AspNetRole();
                aspNetRole.Id = Guid.NewGuid().ToString();
                aspNetRole.Name = roleName;

                db.AspNetRoles.Add(aspNetRole);
                db.SaveChanges();

                response = Global.Global.EnumsSuccess;
            }
            catch(Exception ex)
            {
                response = Global.Global.EnumsError;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> editRole(string oldRoleName, string newRoleName)
        {
            string response = Global.Global.EnumsError;

            try
            {
                AspNetRole aspNetRole = db.AspNetRoles.First(x => x.Name == oldRoleName);
                aspNetRole.Name = newRoleName;

                db.SaveChanges();

                response = Global.Global.EnumsSuccess;
            }
            catch
            {
                response = Global.Global.EnumsError;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteRole(string roleName)
        {
            string response = Global.Global.EnumsError;

            try
            {
                AspNetRole aspNetRole = db.AspNetRoles.First(x => x.Name == roleName);
                db.AspNetRoles.Remove(aspNetRole);
                db.SaveChanges();

                response = Global.Global.EnumsSuccess;
            }
            catch
            {
                response = Global.Global.EnumsError;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
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
