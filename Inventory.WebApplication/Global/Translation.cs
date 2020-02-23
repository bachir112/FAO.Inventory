using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.WebApplication.Global
{
    public class Translation
    {
        public static Dictionary<string, string> LanguageDictionary = new Dictionary<string, string>() {
            {"Home-english", "Home" },
            { "Home-arabic", "قائمة الرئيسة"},
            { "HomeInner-english", "Home Dashboard"},
            { "HomeInner-arabic", "الصفحة الرئيسة"},
            { "Reports-english", "Reports"},
            { "Reports-arabic", "تقارير"},
            { "Transactions-english", "Transactions"},
            { "Transactions-arabic", "عمليات"},
            { "TransactionsHistory-english", "Transactions History"},
            { "TransactionsHistory-arabic", "تاريخ العمليات"},
            { "CreateDelete-english", "Create - Delete"},
            { "CreateDelete-arabic", "اضافة - حذف"},
            { "Pricing-english", "Pricing"},
            { "Pricing-arabic", "التسعير"},
            { "Deteriorated-english", "Useless"},
            { "Deteriorated-arabic", "غير صالح للاستخدام"},
            { "Management-english", "Management"},
            { "Management-arabic", "إدارة"},
            { "Users-english", "Users"},
            { "Users-arabic", "المستخدمين"},
            { "Roles-english", "Roles"},
            { "Roles-arabic", "الأدوار"},
            { "PageManagement-english", "Page Management"},
            { "PageManagement-arabic", "إدارة الصفحات"},
            { "Suppliers-english", "Suppliers"},
            { "Suppliers-arabic", "الموردين"},
            { "Items-english", "stock"},
            { "Items-arabic", "مخزون"},
            { "FilterbyCategory-english", "Filter by Category"},
            { "FilterbyCategory-arabic", "فلتر حسب الفئة"},
            { "Count-english", "Count"},
            { "Count-arabic", "العدد"},
            { "Goto-english", "Go to"},
            { "Goto-arabic", "الذهاب الى"},
            { "RECENTACTIVITIES-english", "RECENT ACTIVITIES"},
            { "RECENTACTIVITIES-arabic", "أنشطة حالية"},

            { "CheckedIn-english", "Checked In"},
            { "CheckedIn-arabic", "أدخل"},

            { "CheckedOut-english", "Checked Out"},
            { "CheckedOut-arabic", "أخرج"},

            { "Thrown-english", "Thrown"},
            { "Thrown-arabic", "رمى"},

            { "Nothing-english", "Nothing"},
            { "Nothing-arabic", "لا شيء"},

            { "To-english", "To"},
            { "To-arabic", "الى"},

            { "ago-english", "ago"},
            { "ago-arabic", "منذ"},



        };

        static public string GetLanguageCookieValue(string cookieName = "language")
        {
            string schoolCookieValue = string.Empty;
            try
            {
                schoolCookieValue = HttpContext.Current.Request.Cookies[cookieName].Value;
            }
            catch (Exception ex)
            {
                schoolCookieValue = "english";
            }

            return schoolCookieValue;
        }

        static public string GetStringValue(string key)
        {
            string result = key;

            string k = key + "-" + GetLanguageCookieValue();

            try
            {
                result = LanguageDictionary[key + "-" + GetLanguageCookieValue()];
            }
            catch(Exception ex)
            {

            }

            return result;
        }
    }
}