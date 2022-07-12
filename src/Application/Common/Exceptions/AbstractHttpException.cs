using System.Net;

namespace Application.Common.Exceptions;

public abstract class AbstractHttpException : Exception
{
    private readonly ApiError _apiError;
    public int StatusCode => _apiError.StatusCode;

    protected AbstractHttpException(HttpStatusCode httpCode, string title, object? details = default)
    {
        _apiError = new ApiError((int)httpCode, title, details);
    }

    public ApiError GetApiError(bool withDetails = true)
        => withDetails
            ? _apiError
            : new ApiError(_apiError.StatusCode, _apiError.Message);
}