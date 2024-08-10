using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Event
{
    public class StockReservedEvent
    {
        public Guid BuyerId { get; set; }
        public Guid OrderId { get; set; }
        public long TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
