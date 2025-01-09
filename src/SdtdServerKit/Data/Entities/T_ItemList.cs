using IceCoffee.SimpleCRUD.OptionalAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Data.Entities
{
    public class T_ItemList
    {
        [PrimaryKey, IgnoreInsert, IgnoreUpdate]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public required string ItemName { get; set; }

        [Column("[Count]")]
        public int Count { get; set; }

        public int Quality { get; set; }

        public int Durability { get; set; }

        public string? Description { get; set; }
    }
}
