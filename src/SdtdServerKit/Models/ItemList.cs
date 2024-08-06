using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Models
{
    public class ItemList
    {
        public string ItemName { get; set; } = null!;

        public int Count { get; set; }

        public int Quality { get; set; }

        public int Durability { get; set; }

        public string? Description { get; set; }
    }
}
