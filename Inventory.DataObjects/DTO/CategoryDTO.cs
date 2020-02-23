using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.DataObjects.EDM;

namespace Inventory.DataObjects.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name_Arabic { get; set; }
        public string Picture { get; set; }
        public int ParentCategory { get; set; }
        public int ItemTypeInCategoryCount { get; set; }
        public int ItemInCategoryCount { get; set; }
    }
}
