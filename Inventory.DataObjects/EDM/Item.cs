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
        public int AvailabilityStatusID { get; set; }
        public int ItemStatusID { get; set; }
        public string LocationInStock { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public int UnitID { get; set; }
        public Nullable<int> UnitAmount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}
