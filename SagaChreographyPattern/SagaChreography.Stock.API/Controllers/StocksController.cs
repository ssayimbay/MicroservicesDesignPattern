using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagaChreography.Stock.API.Models.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaChreography.Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly StockDbContext _stockDbContext;

        public StocksController(StockDbContext stockDbContext)
        {
            _stockDbContext = stockDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stocks = await _stockDbContext.Stocks.ToListAsync();

            return Ok(stocks);
        }
    }
}
