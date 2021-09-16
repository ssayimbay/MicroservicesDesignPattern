using Microsoft.EntityFrameworkCore;

namespace SagaChreography.Stock.API.Models.Contexts
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {

        }

        public DbSet<Stock> Stocks { get; set; }
    }
}
