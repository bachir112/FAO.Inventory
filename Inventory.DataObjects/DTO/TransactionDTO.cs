﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataObjects.DTO
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string OldAvailabilityStatus { get; set; }
        public string NewAvailabilityStatus { get; set; }
        public string Description { get; set; }
        public string StockKeeper { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int Quantity { get; set; }
        public string ToWhom { get; set; }
    }
}