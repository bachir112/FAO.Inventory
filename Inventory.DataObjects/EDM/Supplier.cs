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
    
    public partial class Supplier
    {
        public int Id { get; set; }
        public string Supplier1 { get; set; }
        public Nullable<bool> IsSchool { get; set; }
        public Nullable<int> SchoolID { get; set; }
    
        public virtual School School { get; set; }
    }
}
