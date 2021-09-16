using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SagaChreography.Order.API.Models;
using SagaChreography.Order.API.Models.Contexts;
using SagaChreography.Shared.Events;
using System.Threading.Tasks;

namespace SagaChreography.Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;

        public PaymentCompletedEventConsumer(OrderDbContext orderDbContext, ILogger<PaymentCompletedEventConsumer> logger)
        {
            _orderDbContext = orderDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.Complete;

                await _orderDbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id: {context.Message.OrderId}) status changed: {order.Status}");
            }

            else
            {
                _logger.LogError($"Order (Id: {context.Message.OrderId}) not found.");
            }
        }
    }
}
