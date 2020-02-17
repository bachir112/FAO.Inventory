using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataObjects.DTO
{
    public class ABCItemsDTO
    {
        public string itemName { get; set; }
        public Nullable<decimal> percentageRevenue { get; set; }
        public double percentageQuantity { get; set; }
        public string lineColor { get; set; }
    }
}
