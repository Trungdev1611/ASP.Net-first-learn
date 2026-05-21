using _1.first_learn.database;
using _1.first_learn.Extension;
using _1.first_learn.features.stocks.dtos;
using _1.first_learn.models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace _1.first_learn.features.stocks;

public class StockService
{
    private readonly ApplicationDBContext _context;

    public StockService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<StockDto>> GetAllStocksAsync()
    {
        var stockDtos = await _context.Stocks.AsNoTracking().Select(s => new StockDto
        {
            Id = s.Id,
            Symbol = s.Symbol,
            CompanyName = s.CompanyName,
            Purchase = s.Purchase,
            Divdent = s.Divdent,
            Industry = s.Industry,
            Marketcap = s.Marketcap
        }).ToListAsync();

        return stockDtos;

    }

    public async Task<StockDto> GetStockByIdAsync(int id)
    {
        var stock = await _context.Stocks.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StockDto
            {
                Id = s.Id,
                Symbol = s.Symbol,
                CompanyName = s.CompanyName,
                Purchase = s.Purchase,
                Divdent = s.Divdent,
                Industry = s.Industry,
                Marketcap = s.Marketcap
            }).FirstOrDefaultAsync();

        // Không return null — GlobalExceptionHandler trả 404 + JSON thống nhất
        if (stock is null)
            throw new NotFoundException($"Không tìm thấy cổ phiếu id = {id}");

        return stock;
    }

    public async Task<StockDto> CreateStockAsync(CreateStockDto createStockDto)
    {
        var stockModel = createStockDto.Adapt<Stock>();
        await _context.Stocks.AddAsync(stockModel);
        await _context.SaveChangesAsync();

        return stockModel.Adapt<StockDto>();
    }

    public async Task<StockDto> UpdateStockAsync(int id, UpdateRequestDto updateRequestDTO)
    {
        var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);

        if (stockModel is null)
            throw new NotFoundException($"Không tìm thấy cổ phiếu id = {id}");

        updateRequestDTO.Adapt(stockModel);
        await _context.SaveChangesAsync();

        return stockModel.Adapt<StockDto>();
    }

    public async Task<bool> DeleteAsyncService(int id)
    {
        var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);

        if (stockModel is null)
            throw new NotFoundException($"Không tìm thấy cổ phiếu id = {id}");

        _context.Stocks.Remove(stockModel);
        //Actually delete
        await _context.SaveChangesAsync();
        return true;


    }

    public async Task<List<StocksWithCommentsDTO>> GetAllStocksAndCommentsAsyncService()
    {

        // 1. Tạo một cấu hình riêng cho lệnh này
        var config = new TypeAdapterConfig();

        // 2. Dạy Mapster: Khi gặp Comment, hãy map chuẩn sang CommentDto (Chỉ lấy các trường trong CommentDto)
        config.NewConfig<Comment, CommentDTO>();


        var stocksWithComments = await _context.Stocks
        .AsNoTracking()
        .ProjectToType<StocksWithCommentsDTO>()
        .ToListAsync();

        return stocksWithComments;

    }


}