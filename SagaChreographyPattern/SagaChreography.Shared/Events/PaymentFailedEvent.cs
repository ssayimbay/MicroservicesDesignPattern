using SagaChreography.Shared.Messages;
using System.Collections.Generic;

namespace SagaChreography.Shared.Events
{
    public class PaymentFailedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public string Message { get; set; }
        public List<OrderItemMessage> OrderItemMessages { get; set; }
    }
}
