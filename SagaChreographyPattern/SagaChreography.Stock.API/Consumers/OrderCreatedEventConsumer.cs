using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SagaChreography.Shared.Events;
using SagaChreography.Shared.Settings;
using SagaChreography.Stock.API.Models.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaChreography.Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly StockDbContext _stockDbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(StockDbContext stockDbContext, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _stockDbContext = stockDbContext;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await _stockDbContext.Stocks.AnyAsync(s => s.ProductId == item.ProductId && s.Count >= item.Count));
            }

            if (stockResult.All(s => s.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await _stockDbContext.Stocks.FirstOrDefaultAsync(s => s.ProductId == item.ProductId);

                    if (stock != null)
                    {
                        stock.Count -= item.Count;
                    }

                    await _stockDbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"Stock was reserved for Buyer Id:{context.Message.BuyerId}");

                var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettingsConst.StockReservedEventQueueName}"));

                StockReservedEvent stockReservedEvent = new StockReservedEvent
                {
                    PaymentMessage = context.Message.PaymentMessage,
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    OrderItemMessages = context.Message.OrderItems
                };

                await sendEndPoint.Send(stockReservedEvent);
            }

            else
            {
                await _publishEndpoint.Publish(new StockNotReserverdEvent
                {
                    OrderId = context.Message.OrderId,
                    Message = "Not enough stock."
                });

                _logger.LogInformation($"Not enough stock for Buyer Id:{context.Message.OrderId}");
            }
        }
    }
}
