using System;
using System.Collections.Generic;

namespace SagaChreography.Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string BuyerId { get; set; }

        public Adress Address { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public OrderStatus Status { get; set; }

        public string FailMessage { get; set; }
    }

    public enum OrderStatus
    {
        Suspend,
        Complete,
        Fail
    }
}
