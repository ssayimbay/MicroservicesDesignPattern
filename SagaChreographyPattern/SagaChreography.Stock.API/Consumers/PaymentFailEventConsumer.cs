using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SagaChreography.Shared.Events;
using SagaChreography.Stock.API.Models.Contexts;
using System.Threading.Tasks;

namespace SagaChreography.Stock.API.Consumers
{
    public class PaymentFailEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly StockDbContext _stockDbContext;
        private readonly ILogger<PaymentFailEventConsumer> _logger;

        public PaymentFailEventConsumer(StockDbContext stockDbContext, ILogger<PaymentFailEventConsumer> logger)
        {
            _stockDbContext = stockDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {

            foreach (var orderItem in context.Message.OrderItemMessages)
            {
                var stock = await _stockDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);

                if(stock != null)
                {
                    stock.Count += orderItem.Count;
                    await _stockDbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"Stock  was released for ORDER Id ({context.Message.OrderId})");
   

            }
        }
    }
}
