using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace _1.first_learn.Extension;

/// <summary>
/// Sau khi action chạy xong: tự bọc Ok(data) thành { success, message, data }.
/// Controller chỉ cần return Ok(dto) — không gõ ApiResponse từng action.
/// Lỗi 400/404/500 vẫn do validation factory + GlobalExceptionHandler (không qua filter này).
/// </summary>
public class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // Chỉ API controller (route api/...), không đụng Minimal API MapGet("/")
        if (!context.HttpContext.Request.Path.StartsWithSegments("/api")
            || HasSkipAttribute(context))
        {
            await next();
            return;
        }

        if (context.Result is ObjectResult objectResult
            && objectResult.Value is not null
            && !IsAlreadyApiResponse(objectResult.Value))
        {
            var status = objectResult.StatusCode ?? StatusCodes.Status200OK;
            var message = status == StatusCodes.Status201Created
                ? "Tạo thành công"
                : "Thành công";

            objectResult.Value = WrapSuccess(objectResult.Value, message);
        }

        await next();
    }

    private static bool IsAlreadyApiResponse(object value)
    {
        var type = value.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponse<>);
    }

    private static object WrapSuccess(object value, string message)
    {
        var dataType = value.GetType();
        var apiResponseType = typeof(ApiResponse<>).MakeGenericType(dataType);
        var okMethod = apiResponseType.GetMethod(
            nameof(ApiResponse<object>.Ok),
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [dataType, typeof(string)],
            modifiers: null);

        return okMethod!.Invoke(null, [value, message])!;
    }

    private static bool HasSkipAttribute(ResultExecutingContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(m => m is SkipApiResponseWrapperAttribute))
            return true;

        return context.Controller?.GetType()
            .GetCustomAttributes(typeof(SkipApiResponseWrapperAttribute), inherit: true)
            .Length > 0;
    }
}

/// <summary>
/// Gắn lên action nếu muốn trả JSON thuần, không bọc ApiResponse (hiếm khi cần).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SkipApiResponseWrapperAttribute : Attribute, IFilterMetadata
{
}
