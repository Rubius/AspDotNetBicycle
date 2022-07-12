using System.Net;

namespace Application.Common.Exceptions;

public sealed class ForbiddenException : AbstractHttpException
{
    public ForbiddenException(string title, object? details = null)
        : base(HttpStatusCode.Forbidden, title, details)
    {
    }
}