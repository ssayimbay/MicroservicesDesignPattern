using MassTransit;
using Microsoft.Extensions.Logging;
using SagaChreography.Order.API.Models;
using SagaChreography.Order.API.Models.Contexts;
using SagaChreography.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaChreography.Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(OrderDbContext orderDbContext, ILogger<PaymentFailedEventConsumer> logger)
        {
            _orderDbContext = orderDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _orderDbContext.Orders.FindAsync(context.Message.OrderId);

            if(order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _orderDbContext.SaveChangesAsync();
                _logger.LogInformation($"Order Id: ({context.Message.OrderId}) status changed: {order.Status}");
            }
            else
            {
                _logger.LogError($"Order (Id: {context.Message.OrderId}) not found.");
            }
  
        }
    }
}
