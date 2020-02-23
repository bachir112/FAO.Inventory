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

            { "ItemName-english", "Item Name"},
            { "ItemName-arabic", "الاسم"},

            { "Supplier-english", "Supplier"},
            { "Supplier-arabic", "مزود"},

            { "Price-english", "Price"},
            { "Price-arabic", "السعر"},

            { "ExpiryDate-english", "Expiry Date"},
            { "ExpiryDate-arabic", "تاريخ انتهاء الصلاحية"},

            { "Quantity-english", "Quantity"},
            { "Quantity-arabic", "العدد"},

            { "Amount-english", "Amount"},
            { "Amount-arabic", "كمية"},

            { "ReceivedOn-english", "Received On"},
            { "ReceivedOn-arabic", "وردت في"},

            { "Availability-english", "Availability"},
            { "Availability-arabic", "متاحية"},

            { "LocationInStock-english", "Location in Stock"},
            { "LocationInStock-arabic", "الموقع في المخزون"},

            { "Description-english", "Description"},
            { "Description-arabic", "تفاصيل"},

            { "Status-english", "Status"},
            { "Status-arabic", "الحالة"},

            { "Assign-english", "Assign"},
            { "Assign-arabic", "تعيين"},

            { "ToWhom-english", "To Whom"},
            { "ToWhom-arabic", "إلى من"},

            { "Cancel-english", "Cancel"},
            { "Cancel-arabic", "إلغاء"},

            { "CheckItemsOutOfStock-english", "Check Items Out Of Stock"},
            { "CheckItemsOutOfStock-arabic", "اخراج من المستودع"},

            { "TransactionsQuestion-english", "What would you like to do?"},
            { "TransactionsQuestion-arabic", "ماذا تريد أن تفعل؟"},

            { "TakeItemsInOut-english", "Take Items In - Out of Stock"},
            { "TakeItemsInOut-arabic", "ادخال - اخراج من المستودع"},

            { "ReturnToStock-english", "Return Items To Stock"},
            { "ReturnToStock-arabic", "اعادة الى المستودع"},

            { "TakeOutOfStock-english", "Take Items Out f Stock"},
            { "TakeOutOfStock-arabic", "اخراج من المستودع"},

            { "DeleteOne-english", "Delete One"},
            { "DeleteOne-arabic", "حذف واحد"},

            { "DeleteAll-english", "Delete All"},
            { "DeleteAll-arabic", "حذف الكل"},

            { "ItemsIn-english", "Items in Stock"},
            { "ItemsIn-arabic", "في المخزن"},

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