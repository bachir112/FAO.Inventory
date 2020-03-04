using ClosedXML.Excel;
using Inventory.DataObjects.EDM;
using Inventory.WebApplication.Controllers;
using Inventory.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Inventory.WebApplication.Global
{
    static public class Global
    {
        static public string EnumsError = "error";
        static public string EnumsSuccess = "success";

        static public bool sendEmail(string title, string message, string sendTo)
        {
            bool result = false;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("EssentialDropsLebanon@gmail.com");
                mail.To.Add(sendTo);
                mail.Subject = title;// ;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(@"C:\Users\Hp\Documents\Visual Studio 2019\Projects\FAO.Inventory\Inventory.WebApplication\EmailHTML.html"))
                {
                    body = reader.ReadToEnd();
                }
                //using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/EmailHTML.html")))
                //{
                //    body = reader.ReadToEnd();
                //}
                body = body.Replace("{UserName}", sendTo); //replacing the required things  
                body = body.Replace("{Title}", title);
                body = body.Replace("{message}", message);

                mail.Body = body;

                string AppLocation = "";
                AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                AppLocation = AppLocation.Replace(@"file:\", "");
                string file = AppLocation + @"\ExcelFiles\\Report.xlsx";
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(file); //Attaching File to Mail  
                mail.Attachments.Add(attachment);

                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("EssentialDropsLebanon@gmail.com", "AnAbach@123");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
            }

            return result;
        }

        static public string GetSchoolCookieValue(string cookieName = "schoolDB")
        {
            string schoolCookieValue = string.Empty;
            try
            {
                schoolCookieValue = HttpContext.Current.Request.Cookies[cookieName].Value;
            }
            catch(Exception ex)
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Session.Abandon();
            }

            return schoolCookieValue;
        }

        static public void ExportDataSetToExcel<T>(this List<T> list)
        {
            string AppLocation = "";
            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            AppLocation = AppLocation.Replace(@"file:\", "");
            string file = AppLocation + @"\ExcelFiles\Report.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                DataTable dataTable = ToDataTable(list);
                wb.Worksheets.Add(dataTable);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.SaveAs(file);
            }
        }

        static public DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        static public bool isAllowed(string userID, string pageName)
        {
            bool result = false;

            try
            {
                string roleName = "";
                using (var context = new ApplicationDbContext(Global.GetSchoolCookieValue()))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    var user = userManager.FindById(userID);
                    roleName = userManager.GetRoles(user.Id).FirstOrDefault();
                }

                using (var db = new InventoryEntities(Global.GetSchoolCookieValue()))
                {
                    result = db.PageManagements.Where(x => x.RoleName == roleName &&
                                                           x.PageName == pageName &&
                                                           x.Allowed
                                                     )
                                                .Select(x => x)
                                                .Count() > 0;
                }
            }
            catch(Exception ex)
            {

            }
            
            return result;
        }

        static public List<PageManagement> AllowedPages(string userID)
        {
            List<PageManagement> result = new List<PageManagement>();

            try
            {
                string roleName = "";
                using (var context = new ApplicationDbContext(Global.GetSchoolCookieValue()))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    var user = userManager.FindById(userID);
                    roleName = userManager.GetRoles(user.Id).FirstOrDefault();
                }

                using (var db = new InventoryEntities(Global.GetSchoolCookieValue()))
                {
                    result = db.PageManagements.Where(x => x.RoleName == roleName && x.Allowed).Select(x => x).ToList();
                }
            }
            catch(Exception ex)
            {

            }

            return result;
        }


        //static async public Task<bool> createNotification(string action, string message)
        //{
        //    bool result = false;

        //    try
        //    {
        //        StringBuilder httpRouteNotification = new StringBuilder();
        //        httpRouteNotification.Append(Global.baseUrl);
        //        httpRouteNotification.Append("api/Notifications/Create");

        //        await httpRouteNotification.ToString()
        //            .PostJsonAsync(new Notification
        //            {
        //                Action = action,
        //                Message = message
        //            });

        //        result = true;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }

        //    return result;
        //}

        //static async public Task<List<Notification>> getNotifications(string userID)
        //{
        //    List<Notification> result = new List<Notification>();

        //    using (var client = new HttpClient())
        //    {
        //        StringBuilder httpRouteNotification = new StringBuilder();
        //        httpRouteNotification.Append(Global.baseUrl);
        //        httpRouteNotification.Append("api/Notifications/Get");
        //        httpRouteNotification.Append("?");
        //        httpRouteNotification.AppendFormat("userID={0}", userID);

        //        var response = await client.GetAsync(httpRouteNotification.ToString());
        //        if (response.IsSuccessStatusCode)
        //        {
        //            result = await response.Content.ReadAsAsync<List<Notification>>();
        //        }
        //    }

        //    return result;
        //}
    }
}