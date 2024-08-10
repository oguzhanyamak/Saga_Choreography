using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class OrderItemMessage
    {
        public Guid ProductId { get; set; }
        public uint Count { get; set; }
        public long Price { get; set; }
    }
}
