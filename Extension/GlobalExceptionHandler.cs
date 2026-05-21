using Microsoft.AspNetCore.Diagnostics;

namespace _1.first_learn.Extension;

/// <summary>
/// Bắt mọi exception chưa xử lý trong pipeline (từ Controller/Service).
/// Khác với InvalidModelStateResponseFactory — cái kia chỉ bắt lỗi validation DTO ([Required], [Range]...).
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Map từng loại exception sang status + body thống nhất
        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                ApiResponse<object>.Fail(ex.Message)),

            BadRequestException ex => (
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.Fail(ex.Message, ex.Errors)),

            // Lỗi không mong đợi (DB, null reference...) — không lộ chi tiết ra client
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Fail("Đã xảy ra lỗi hệ thống"))
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true; // đã xử lý xong, không cần handler khác
    }
}
