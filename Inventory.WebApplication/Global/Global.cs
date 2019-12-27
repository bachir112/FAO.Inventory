using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventory.WebApplication.Global
{
    public class Global
    {
        static public string EnumsError = "error";
        static public string EnumsSuccess = "success";

        static public bool sendEmail(string title, string body, string sendTo)
        {
            bool result = false;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("EssentialDropsLebanon@gmail.com");
                mail.From = new MailAddress("hello@mabsoot.com");
                mail.To.Add(sendTo);
                mail.Subject = title;// ;

                mail.Body = body;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("EssentialDropsLebanon@gmail.com", "AnAbach@123");

                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                result = true;
            }
            catch
            {
                result = false;
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