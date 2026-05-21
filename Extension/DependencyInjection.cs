using Microsoft.AspNetCore.Mvc;

namespace _1.first_learn.Extension;

public static class DependencyInjection
{
    /// <summary>
    /// Lớp 1 — Lỗi validation: khi body/query không pass [Required], [Range]... trên DTO.
    /// [ApiController] tự trả 400; factory này đổi body sang ApiResponse thống nhất.
    /// </summary>
    public static IServiceCollection AddCustomValidationErrorHandling(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var cleanErrors = context.ModelState
                    .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key.Replace("$.", ""),
                        kvp => kvp.Value!.Errors.First().ErrorMessage);

                var errorResponse = ApiResponse<object>.Fail(
                    "Dữ liệu gửi lên không hợp lệ!",
                    cleanErrors);

                return new BadRequestObjectResult(errorResponse);
            };
        });

        return services;
    }

    /// <summary>
    /// Lớp 2 — Exception từ Service (NotFoundException, BadRequestException, lỗi 500...).
    /// Đăng ký handler; phải gọi app.UseExceptionHandler() trong Program.cs.
    /// </summary>
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>(); //đăng kí GlobalExceptionHandler vào đây
        services.AddProblemDetails(); // pipeline chuẩn ASP.NET, hỗ trợ UseExceptionHandler (phải có dòn này mới chạy)
        return services;
    }
}
