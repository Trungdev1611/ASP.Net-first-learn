using _1.first_learn.Extension;
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
    public async Task<ActionResult<ApiResponse<List<StockDto>>>> GetAllStocks()
    {
        var stockDtos = await _stockService.GetAllStocksAsync();
        // Trước đây return Ok(stockDtos) → client nhận mảng thuần [...]
        // Bọc ApiResponse → client nhận { success, message, data: [...] }
        return Ok(ApiResponse<List<StockDto>>.Ok(stockDtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StockDto>>> GetStockById([FromRoute] int id)
    {
        var stock = await _stockService.GetStockByIdAsync(id);
        return Ok(ApiResponse<StockDto>.Ok(stock));
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<StockDto>>> CreateStock([FromBody] CreateStockDto createStockDto)
    {
        var createdStockDto = await _stockService.CreateStockAsync(createStockDto);
        var body = ApiResponse<StockDto>.Ok(createdStockDto, "Tạo cổ phiếu thành công");
        return CreatedAtAction(nameof(GetStockById), new { id = createdStockDto.Id }, body);
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<ApiResponse<StockDto>>> UpdateStock(
        [FromRoute] int id,
        [FromBody] UpdateRequestDto updateRequestDto)
    {
        var updatedStockDto = await _stockService.UpdateStockAsync(id, updateRequestDto);
        return Ok(ApiResponse<StockDto>.Ok(updatedStockDto, "Cập nhật thành công"));
    }
}
