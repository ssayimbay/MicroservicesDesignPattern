using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaChreography.Order.API.Dtos;
using SagaChreography.Order.API.Models;
using SagaChreography.Order.API.Models.Contexts;
using SagaChreography.Shared.Events;
using SagaChreography.Shared.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SagaChreography.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreateDto.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Adress
                {
                    Line = orderCreateDto.AdressDto.Line,
                    Province = orderCreateDto.AdressDto.Province,
                    District = orderCreateDto.AdressDto.District
                },
                CreatedDate = DateTime.Now
            };

            orderCreateDto.OrderItemDtos.ForEach(orderItemDto =>
            {
                newOrder.Items.Add(new OrderItem() { Price = orderItemDto.Price, ProductId = orderItemDto.ProductId, Count = orderItemDto.Count });
            });

            await _context.AddAsync(newOrder);

            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = orderCreateDto.BuyerId,
                OrderId = newOrder.Id,
                PaymentMessage = new PaymentMessage
                {
                    CardName = orderCreateDto.PaymentDto.CardName,
                    CardNumber = orderCreateDto.PaymentDto.CardNumber,
                    Expiration = orderCreateDto.PaymentDto.Expiration,
                    CVV = orderCreateDto.PaymentDto.CVV,
                    TotalPrice = orderCreateDto.OrderItemDtos.Sum(x => x.Price * x.Count)
                }
            };

            orderCreateDto.OrderItemDtos.ForEach(orderItemDto =>
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage { Count = orderItemDto.Count, ProductId = orderItemDto.ProductId });
            });

            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }

    }
}
