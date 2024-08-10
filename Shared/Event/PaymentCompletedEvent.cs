using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Event
{
    public class PaymentCompletedEvent
    {
        public Guid OrderId { get; set; }
    }
}
