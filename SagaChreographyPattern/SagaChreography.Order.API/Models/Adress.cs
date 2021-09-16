using Microsoft.EntityFrameworkCore;

namespace SagaChreography.Order.API.Models
{
    [Owned]
    public class Adress
    {
        public string Line { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }
}
