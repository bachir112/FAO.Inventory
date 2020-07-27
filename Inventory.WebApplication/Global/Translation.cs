using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.WebApplication.Global
{
    public class Translation
    {
        public static Dictionary<string, string> LanguageDictionary = new Dictionary<string, string>() {
            { "Home-english", "Home" },
            { "Home-arabic", "قائمة الرئيسة"},

            { "HomeInner-english", "Home Dashboard"},
            { "HomeInner-arabic", "الصفحة الرئيسة"},

            { "GeneratedReports-english", "Generated Reports"},
            { "GeneratedReports-arabic", "تقارير مولدة"},

            { "Reports-english", "Reports"},
            { "Reports-arabic", "تقارير"},

            { "Generate-english", "Generate"},
            { "Generate-arabic", "انشاء"},

            { "Scale-english", "Scale"},
            { "Scale-arabic", "مقياس"},

            { "SendAsEmail-english", "Send as email"},
            { "SendAsEmail-arabic", "إرسال بالبريد الإلكتروني"},

            { "ReportSettings-english", "Report Settings"},
            { "ReportSettings-arabic", "اعدادات التقرير"},

            { "ReportName-english", "Report Name"},
            { "ReportName-arabic", "اسم التقرير"},

            { "Action-english", "Action"},
            { "Action-arabic", "عمل"},

            { "MinQuantity-english", "Min. Quantity"},
            { "MinQuantity-arabic", "الحد الأدنى من الكمية"},

            { "MaxQuantity-english", "Max. Quantity"},
            { "MaxQuantity-arabic", "الحد الأقصى من الكمية"},

            { "MinPrice-english", "Min. Price"},
            { "MinPrice-arabic", "سعر الحد الأدنى"},

            { "MaxPrice-english", "Max. Price"},
            { "MaxPrice-arabic", "السعر الاقصى"},

            { "QuantityIn-english", "Quantity In"},
            { "QuantityIn-arabic", "الكمية في"},

            { "MaintenanceAmount-english", "Maintenance Amount"},
            { "MaintenanceAmount-arabic", "مبلغ الصيانة"},

            { "MaintenancePrice-english", "Maintenance Amount"},
            { "MaintenancePrice-arabic", "مبلغ الصيانة"},

            { "MaintenanceReport-english", "Maintenance Report"},
            { "MaintenanceReport-arabic", "تقرير صيانة"},

            { "Consumable-english", "Consumable"},
            { "Consumable-arabic", "مستهلك"},

            { "Expandables-english", "Consumable"},
            { "Expandables-arabic", "مستهلك"},

            { "Available-english", "Available"},
            { "Available-arabic", "متوفر"},

            { "From-english", "From"},
            { "From-arabic", "من"},

            { "QuantityOut-english", "Quantity Out"},
            { "QuantityOut-arabic", "الكمية خارج"},

            { "AvailableQuantity-english", "Available Quantity"},
            { "AvailableQuantity-arabic", "الكمية المتوفرة"},
            
            { "Transactions-english", "Transactions"},
            { "Transactions-arabic", "عمليات"},

            { "TransactionsHistory-english", "Transactions History"},
            { "TransactionsHistory-arabic", "تاريخ العمليات"},

            { "CreateDelete-english", "Create - Delete"},
            { "CreateDelete-arabic", "اضافة - حذف"},

            { "Pricing-english", "Pricing"},
            { "Pricing-arabic", "التسعير"},

            { "ItemsPrices-english", "Items Prices"},
            { "ItemsPrices-arabic", "بنود الأسعار"},

            { "ItemInGroup-english", "Item In Group"},
            { "ItemInGroup-arabic", "القطعة في المجموعة"},
            
            { "Deteriorated-english", "Deteriorated"},
            { "Deteriorated-arabic", "غير صالح للاستخدام"},

            { "Management-english", "Management"},
            { "Management-arabic", "إدارة"},

            { "Close-english", "Close"},
            { "Close-arabic", "غلق"},

            { "Reset-english", "Reset"},
            { "Reset-arabic", "إعادة تعيين"},

            { "ResetPassword-english", "Reset Password"},
            { "ResetPassword-arabic", "إعادة تعيين كلمة المرور"},

            { "YouWillReceiveAnEmail-english", "You will receive an email with the new password."},
            { "YouWillReceiveAnEmail-arabic", "ستتلقى رسالة بريد إلكتروني تحتوي على كلمة المرور الجديدة."},

            { "PleaseCheckYourInbox-english", "Please check your inbox."},
            { "PleaseCheckYourInbox-arabic", "يرجى التحقق من البريد الوارد الخاص بك."},

            { "Users-english", "Users"},
            { "Users-arabic", "المستخدمين"},

            { "User-english", "User"},
            { "User-arabic", "المستخدم"},

            { "CreateUser-english", "Create User"},
            { "CreateUser-arabic", "إنشاء مستخدم"},

            { "CheckedInStock-english", "Checked In Stock"},
            { "CheckedInStock-arabic", "دخل الى المستودع"},

            { "FromDate-english", "From Date"},
            { "FromDate-arabic", "من تاريخ"},

            { "ToDate-english", "To Date"},
            { "ToDate-arabic", "الى تاريخ"},

            { "SelectDateRange-english", "Select date range"},
            { "SelectDateRange-arabic", "اختر نطاق التاريخ"},

            { "Search-english", "Search"},
            { "Search-arabic", "بحث"},

            { "Set-english", "Set"},
            { "Set-arabic", "ثحديد"},
            
            { "AssignPrice-english", "Assign Price"},
            { "AssignPrice-arabic", "تعيين السعر"},
            
            { "Email-english", "Email"},
            { "Email-arabic", "البريد الإلكتروني"},

            { "AreYouSureYouWantToDeleteThisItemFromTheStock-english", "Are you sure you want to delete this item from the stock?"},
            { "AreYouSureYouWantToDeleteThisItemFromTheStock-arabic", "هل تريد بالتأكيد حذف هذا العنصر من المخزون؟"},

            { "DeleteQuantity-english", "How much do you want to delete?"},
            { "DeleteQuantity-arabic", "كم تريد الحذف من المخزون؟"},

            { "Username-english", "Username"},
            { "Username-arabic", "اسم المستخدم"},

            { "Password-english", "Password"},
            { "Password-arabic", "كلمه السر"},

            { "RetypeYourPassword-english", "Re-type Your Password"},
            { "RetypeYourPassword-arabic", "أعد كتابة كلمة مرورك"},

            { "Create-english", "Create"},
            { "Create-arabic", "خلق"},

            { "NewItemInStock-english", "New Item in Stock"},
            { "NewItemInStock-arabic", "عنصر جديد في المخزون"},

            { "Categories-english", "Categories"},
            { "Categories-arabic", "فئات"},

            { "Details-english", "Details"},
            { "Details-arabic", "تفاصيل"},

            { "Location-english", "Location"},
            { "Location-arabic", "موقع"},

            { "Name-english", "Name"},
            { "Name-arabic", "اسم"},
                        
            { "Next-english", "Next"},
            { "Next-arabic", "التالى"},

            { "Previous-english", "Previous"},
            { "Previous-arabic", "السابق"},

            { "Update-english", "Update"},
            { "Update-arabic", "تحديث"},

            { "AddOrDeleteItems-english", "Add or delete items"},
            { "AddOrDeleteItems-arabic", "إضافة أو حذف"},

            { "WhatCategoryDoesThisItemBelongTo-english", "What category does this item belong to?"},
            { "WhatCategoryDoesThisItemBelongTo-arabic", "ما الفئة التي ينتمي إليها؟"},

            { "ItemsUnderThisCategory-english", "Items under this category"},
            { "ItemsUnderThisCategory-arabic", "العناصر تحت هذه الفئة"},

            { "SelectFromThisList-english", "Select from this list"},
            { "SelectFromThisList-arabic", "اختر من هذه القائمة"},

            { "CouldntFindIt-english", "Couldn't find it?"},
            { "CouldntFindIt-arabic", "لا يمكن العثور عليه؟"},

            { "AddItHere-english", "Add it here"},
            { "AddItHere-arabic", "أضفه هنا"},

            { "Add-english", "Add"},
            { "Add-arabic", "إضافة"},

            { "WriteDownTheFinalDetails-english", "Write down the final details"},
            { "WriteDownTheFinalDetails-arabic", "اكتب التفاصيل النهائية"},
            

            { "AreYouSureYouWantToDeleteThisUser-english", "Are you sure you want to delete this user?"},
            { "AreYouSureYouWantToDeleteThisUser-arabic", "هل أنت متأكد أنك تريد حذف هذا المستخدم؟"},

            { "AreYouSureYouWantToDeleteThisSupplier-english", "Are you sure you want to delete this supplier?"},
            { "AreYouSureYouWantToDeleteThisSupplier-arabic", "هل أنت متأكد أنك تريد حذف هذا المورد؟"},

            { "NewPassword-english", "New password"},
            { "NewPassword-arabic", "كلمة مرور جديدة"},

            { "ChangeUserPassword-english", "Change user's password"},
            { "ChangeUserPassword-arabic", "تغيير كلمة مرور المستخدم"},

            { "BackToList-english", "Back to List"},
            { "BackToList-arabic", "الرجوع للقائمة"},

            { "EnterYourAccountDetailsBelow-english", "Enter your account details below"},
            { "EnterYourAccountDetailsBelow-arabic", "أدخل تفاصيل حسابك أدناه"},

            { "EnterYourPersonalDetailsBelow-english", "Enter your personal details below"},
            { "EnterYourPersonalDetailsBelow-arabic", "أدخل بياناتك الشخصية أدناه"},

            { "FullName-english", "Full Name"},
            { "FullName-arabic", "الاسم الكامل"},

            { "List-english", "List"},
            { "List-arabic", "قائمة"},

            { "Roles-english", "Roles"},
            { "Roles-arabic", "الأدوار"},

            { "Role-english", "Roles"},
            { "Role-arabic", "الدور"},

            { "AreYouSureYouWantToDeleteThisRole-english", "Are you sure you want to delete this role?"},
            { "AreYouSureYouWantToDeleteThisRole-arabic", "هل أنت متأكد أنك تريد حذف هذا الدور؟"},

            { "Phone-english", "Phone"},
            { "Phone-arabic", "الهاتف"},

            { "LastLogin-english", "Last login"},
            { "LastLogin-arabic", "آخر تسجيل دخول"},

            { "PageManagement-english", "Page Management"},
            { "PageManagement-arabic", "إدارة الصفحات"},

            { "ManagePagesByRoles-english", "Manage Pages By Roles"},
            { "ManagePagesByRoles-arabic", "إدارة الصفحات حسب الأدوار"},

            { "Page-english", "Page"},
            { "Page-arabic", "صفحة"},
            
            { "PageName-english", "Page name"},
            { "PageName-arabic", "اسم الصفحة"},

            { "Allowed-english", "Allowed"},
            { "Allowed-arabic", "سماح"},

            { "NotAllowed-english", "Allowed"},
            { "NotAllowed-arabic", "غير مسموح"},

            { "NoSettingsAssigned-english", "No Settings Assigned"},
            { "NoSettingsAssigned-arabic", "لا توجد إعدادات مخصصة"},

            { "AuthorizationTable-english", "Authorization table"},
            { "AuthorizationTable-arabic", "جدول التفويض"},

            { "Authorization-english", "Authorization"},
            { "Authorization-arabic", "تفويض"},

            { "ByRole-english", "By role"},
            { "ByRole-arabic", "حسب الدور"},

            { "IsASchool-english", "Is a School"},
            { "IsASchool-arabic", "هل هو مدرسة؟"},

            { "ApproveTransfer-english", "Approve Transfer"},
            { "ApproveTransfer-arabic", "الموافقة على النقل"},

            { "Approve-english", "Approve"},
            { "Approve-arabic", "الموافقة"},

            { "Suppliers-english", "Suppliers"},
            { "Suppliers-arabic", "الموردين"},

            { "Items-english", "stock"},
            { "Items-arabic", "مخزن"},

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

            { "Category-english", "Category"},
            { "Category-arabic", "الفئة"},

            { "ConsumableNonConsumable-english", "Consumable / Non-Consumable"},
            { "ConsumableNonConsumable-arabic", "مستهلكات / غير مستهلكات"},

            { "DateIn-english", "Date in"},
            { "DateIn-arabic", "تاريخ الدخول"},

            { "Code-english", "Code"},
            { "Code-arabic", "رمز"},

            { "DateTransfered-english", "Date Transfered"},
            { "DateTransfered-arabic", "تاريخ النقل"},

            { "ReasonOfTransfer-english", "Reason of transfer"},
            { "ReasonOfTransfer-arabic", "سبب النقل"},
            
            { "Source-english", "Source"},
            { "Source-arabic", "مصدر"},

            { "DateOut-english", "Date out"},
            { "DateOut-arabic", "تاريخ الخروج"},

            { "ReturnDate-english", "Return date"},
            { "ReturnDate-arabic", "تاريخ العودة"},

            { "ShouldReturnOn-english", "Should return on"},
            { "ShouldReturnOn-arabic", "يجب أن يعود في تاريخ"},

            { "CostPerItem-english", "Cost per item"},
            { "CostPerItem-arabic", "التكلفة لكل بند"},

            { "GrossTotal-english", "Gross total"},
            { "GrossTotal-arabic", "المجموع الكلي"},
            
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
            
            { "HowMany-english", "How many?"},
            { "HowMany-arabic", "كم العدد؟"},

            { "LocationInStock-english", "Location in Stock"},
            { "LocationInStock-arabic", "الموقع في المخزون"},
            
            { "Unit-english", "Unit"},
            { "Unit-arabic", "وحدة"},

            { "Description-english", "Description"},
            { "Description-arabic", "تفاصيل"},

            { "Status-english", "Status"},
            { "Status-arabic", "الحالة"},

            { "Assign-english", "Assign"},
            { "Assign-arabic", "تعيين"},

            { "ToWhom-english", "To Whom"},
            { "ToWhom-arabic", "إلى من"},

            { "FromStatus-english", "From Status"},
            { "FromStatus-arabic", "من"},

            { "ToStatus-english", "To Status"},
            { "ToStatus-arabic", "الى"},

            { "StockKeeper-english", "StockKeeper"},
            { "StockKeeper-arabic", "أمين المستودع"},

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

            { "TakeOutOfStock-english", "Take Items Out of Stock"},
            { "TakeOutOfStock-arabic", "اخراج من المستودع"},

            { "DeleteOne-english", "Delete One"},
            { "DeleteOne-arabic", "حذف واحد"},

            { "DeleteAll-english", "Delete All"},
            { "DeleteAll-arabic", "حذف الكل"},

            { "ItemsIn-english", "Items in Stock"},
            { "ItemsIn-arabic", "في المخزن"},

            { "AddNewItemsToStock-english", "Add new items to stock"},
            { "AddNewItemsToStock-arabic", "اضافة الى المخزن"},

            { "DeleteItemsFromStock-english", "Delete items in stock"},
            { "DeleteItemsFromStock-arabic", "ازالة من المخزن"},

            { "CreateNew-english", "Create New"},
            { "CreateNew-arabic", "خلق جديد إبداع جديد"},

            { "Edit-english", "Edit"},
            { "Edit-arabic", "تعديل"},

            { "Delete-english", "Delete"},
            { "Delete-arabic", "حذف"},

            { "ChangePassword-english", "Change Password"},
            { "ChangePassword-arabic", "تغيير كلمة السر"},

            { "Value-english", "Value"},
            { "Value-arabic", "القيمة"},


        }; 

        static public string GetLanguageCookieValue(string cookieName = "language")
        {
            string schoolCookieValue = string.Empty;
            if(HttpContext.Current.Request.Cookies[cookieName] == null)
            {
                schoolCookieValue = "english";
            }
            else
            {
                schoolCookieValue = HttpContext.Current.Request.Cookies[cookieName].Value;
            }

            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //}

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