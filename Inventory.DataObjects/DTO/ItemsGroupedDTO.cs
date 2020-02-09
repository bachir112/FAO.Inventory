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
        public int AvailabilityStatusID { get; set; }
        public string AvailabilityStatus { get; set; }
        public int ItemStatusID { get; set; }
        public string ItemStatus { get; set; }
        public string LocationInStock { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public int UnitID { get; set; }
        public string Unit { get; set; }
        public Nullable<int> UnitAmount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public int SupplierID { get; set; }
        public string Supplier { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> QuantityIn { get; set; }
        public Nullable<int> QuantityOut { get; set; }
        public Nullable<System.DateTime> ReceivedOn { get; set; }
    }
}
