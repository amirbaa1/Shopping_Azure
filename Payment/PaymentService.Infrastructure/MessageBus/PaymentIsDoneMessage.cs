using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.MessageBus
{
    public class PaymentIsDoneMessage : BaseMessage
    {
        public Guid orderId { get; set; }
    }
}
