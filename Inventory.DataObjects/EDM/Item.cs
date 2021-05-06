//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory.DataObjects.EDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name_Arabic { get; set; }
        public int AvailabilityStatusID { get; set; }
        public int ItemStatusID { get; set; }
        public string LocationInStock { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public int UnitID { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> UnitAmount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> MaintenancePrice { get; set; }
        public Nullable<bool> Expandable { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> ReceivedOn { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string PendingTransferApproval { get; set; }
        public Nullable<int> SchoolID { get; set; }
    }
}
