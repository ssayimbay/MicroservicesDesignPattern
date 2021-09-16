using SagaChreography.Shared.Messages;
using System.Collections.Generic;

namespace SagaChreography.Shared.Events
{
    public class StockReservedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public PaymentMessage PaymentMessage { get; set; }
        public List<OrderItemMessage> OrderItemMessages { get; set; } = new List<OrderItemMessage>();
    }
}
