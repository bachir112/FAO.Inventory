using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataObjects.DTO
{
    public class ItemsGroupedDTO
    {
        public Nullable<int> GroupedId { get; set; }
        public string ItemsIDs { get; set; }
        public string Name { get; set; }
        public string Name_Arabic { get; set; }
        public int AvailabilityStatusID { get; set; }
        public string AvailabilityStatus { get; set; }
        public string AvailabilityStatus_Arabic { get; set; }
        public int ItemStatusID { get; set; }
        public string ItemStatus { get; set; }
        public string LocationInStock { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public int UnitID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public string SchoolName { get; set; }
        public Nullable<bool> Expandable { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> UnitAmount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public string Category_Arabic { get; set; }
        public int SupplierID { get; set; }
        public string Supplier { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> MaintenancePrice { get; set; }
        public Nullable<int> QuantityIn { get; set; }
        public Nullable<int> QuantityOut { get; set; }
        public Nullable<System.DateTime> ReceivedOn { get; set; }
        public Nullable<System.DateTime> DateOn { get; set; }
    }
}
