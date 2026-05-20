
using _1.first_learn.database;
using _1.first_learn.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1.first_learn.features.stocks;

[Route("api/stocks")]
[ApiController]
    public class StockController:ControllerBase
    {
        private readonly ApplicationDBContext  _context; //DI, connect to DB

        // Inject DbContext để thao tác với Postgres
        public StockController(ApplicationDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Stock>>> GetAllStocks() {
            var stocks = await _context.Stocks.ToListAsync();
            return Ok(stocks);
        }
    }