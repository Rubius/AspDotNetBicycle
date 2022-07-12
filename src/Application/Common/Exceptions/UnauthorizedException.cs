using System.Net;

namespace Application.Common.Exceptions;

public sealed class UnauthorizedException: AbstractHttpException
{
    public UnauthorizedException(string title, object? details = null)
        : base(HttpStatusCode.Unauthorized, title, details) { }
}