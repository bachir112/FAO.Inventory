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
    
    public partial class ReportSetting
    {
        public int Id { get; set; }
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public string ReceivedByUsers { get; set; }
        public bool DailyBasis { get; set; }
        public bool WeeklyBasis { get; set; }
        public bool MonthlyBasis { get; set; }
        public bool YearlyBasis { get; set; }
        public string SpecificDates { get; set; }
        public bool QueryBasis { get; set; }
        public Nullable<int> QueryID { get; set; }
        public Nullable<System.DateTime> LastSent { get; set; }
        public Nullable<int> SchoolID { get; set; }
    }
}
