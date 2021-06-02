using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataObjects.DTO
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string ItemID { get; set; }
        public string SerialNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemName_Arabic { get; set; }
        public string Category { get; set; }
        public string Category_Arabic { get; set; }
        public int? OldAvailabilityStatusID { get; set; }
        public string OldAvailabilityStatus { get; set; }
        public string OldAvailabilityStatus_Arabic { get; set; }
        public int? NewAvailabilityStatusID { get; set; }
        public string NewAvailabilityStatus { get; set; }
        public string NewAvailabilityStatus_Arabic { get; set; }
        public string Description { get; set; }
        public string StockKeeper { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> UnitAmount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int Quantity { get; set; }
        public int QuantityAvailable { get; set; }
        public string ToWhom { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> NewSchoolID { get; set; }
        public string ToSchoolName { get; set; }
    }
}
