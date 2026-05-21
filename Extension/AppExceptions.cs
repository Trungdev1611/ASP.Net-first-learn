namespace _1.first_learn.Extension;

/// <summary>
/// Ném trong Service khi không tìm thấy bản ghi.
/// GlobalExceptionHandler sẽ map sang HTTP 404 + body ApiResponse.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>
/// Lỗi nghiệp vụ / input không hợp lệ do code kiểm tra (không phải DataAnnotations).
/// GlobalExceptionHandler map sang HTTP 400.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, Dictionary<string, string> errors)
        : base(message)
    {
        Errors = errors;
    }

    public Dictionary<string, string>? Errors { get; }
}
