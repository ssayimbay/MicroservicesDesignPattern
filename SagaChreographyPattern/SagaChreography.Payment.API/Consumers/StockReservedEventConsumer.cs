using MassTransit;
using Microsoft.Extensions.Logging;
using SagaChreography.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaChreography.Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var fakeBalance = 3000m;

            if (fakeBalance > context.Message.PaymentMessage.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice} TL was withdrawn from credit card for user id: {context.Message.BuyerId} ");

                await _publishEndpoint.Publish(new PaymentCompletedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId
                });
            }

            else
            {
                _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice} TL was not withdrawn from credit card for user id: {context.Message.BuyerId} ");
                await _publishEndpoint.Publish(new PaymentFailedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Not enought balance.",
                    OrderItemMessages = context.Message.OrderItemMessages
                    
                    
                });
            }
        }
    }
}
