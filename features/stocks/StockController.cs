using _1.first_learn.features.stocks.dtos;
using Microsoft.AspNetCore.Mvc;

namespace _1.first_learn.features.stocks;

[Route("api/stocks")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockDto>>> GetAllStocks()
    {
        return Ok(await _stockService.GetAllStocksAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockDto>> GetStockById([FromRoute] int id)
    {
        return Ok(await _stockService.GetStockByIdAsync(id));
    }

    [HttpPost("create")]
    public async Task<ActionResult<StockDto>> CreateStock([FromBody] CreateStockDto createStockDto)
    {
        var created = await _stockService.CreateStockAsync(createStockDto);
        return CreatedAtAction(nameof(GetStockById), new { id = created.Id }, created);
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<StockDto>> UpdateStock(
        [FromRoute] int id,
        [FromBody] UpdateRequestDto updateRequestDto)
    {
        return Ok(await _stockService.UpdateStockAsync(id, updateRequestDto));
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteStockAsync([FromRoute] int id)
    {
        return Ok(await _stockService.DeleteAsyncService(id));
    }

    // GET api/stocks/comments (không dùng "/comments" — dấu / đầu = route tuyệt đối /comments)
    [HttpGet("comments")]
    public async Task<ActionResult<List<StocksWithCommentsDTO>>> GetAllStocksAndCommentsAsync()
    {
        return Ok(await _stockService.GetAllStocksAndCommentsAsyncService());
    }


}
