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
    
    public partial class ReportQuery
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> MinimumQuantity { get; set; }
        public Nullable<int> MaximumQuantity { get; set; }
        public Nullable<int> MinimumPrice { get; set; }
        public Nullable<int> MaximumPrice { get; set; }
        public Nullable<int> AvailabilityStatusID { get; set; }
        public Nullable<int> ReportID { get; set; }
    }
}
