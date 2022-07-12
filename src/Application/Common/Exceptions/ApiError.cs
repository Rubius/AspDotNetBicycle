namespace Application.Common.Exceptions;

public class ApiError
{
    public ApiError(int statusCode, string message, object? details = default)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    public int StatusCode { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public object? Details { get; private set; }
    public object? Extensions { get; set; }
}