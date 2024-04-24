using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Shared.Models
{
    public interface IPlayer
    {
        public int EntityId { get; set; }
        public string PlatformId { get; set; }
        public string PlayerName { get; set; }
    }
}
