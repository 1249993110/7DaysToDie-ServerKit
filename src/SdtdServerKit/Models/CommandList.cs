using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Models
{
    public class CommandList
    {
        public string Command { get; set; } = null!;

        public bool InMainThread { get; set; }

        public string? Description { get; set; }
    }
}
