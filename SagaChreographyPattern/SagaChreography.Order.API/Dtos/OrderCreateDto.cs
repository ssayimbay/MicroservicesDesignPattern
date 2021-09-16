using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaChreography.Order.API.Dtos
{
    public class OrderCreateDto
    {
        public string BuyerId { get; set; }
        public PaymentDto PaymentDto { get; set; }
        public List<OrderItemDto> OrderItemDtos { get; set; } = new List<OrderItemDto>();
        public AdressDto AdressDto { get; set; }
    }
}
