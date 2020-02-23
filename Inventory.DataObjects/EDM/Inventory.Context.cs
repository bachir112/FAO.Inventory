﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class InventoryEntities : DbContext
    {
        public InventoryEntities()
            : base("name=InventoryEntities")
        {
        }
        public InventoryEntities(string schoolDB)
            : base("name=InventoryEntities_" + schoolDB)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AvailabilityStatu> AvailabilityStatus { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ItemsSearchValue> ItemsSearchValues { get; set; }
        public virtual DbSet<ItemStatu> ItemStatus { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<PageManagement> PageManagements { get; set; }
        public virtual DbSet<ReportQuery> ReportQueries { get; set; }
        public virtual DbSet<ReportSetting> ReportSettings { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<TransactionsReminder> TransactionsReminders { get; set; }
    }
}
