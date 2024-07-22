using IceCoffee.SimpleCRUD.OptionalAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Data.Entities
{
    public class T_VipGiftCommand
    {
        [PrimaryKey]
        public string VipGiftId { get; set; }

        [PrimaryKey]
        public int CommandId { get; set; }
    }
}
