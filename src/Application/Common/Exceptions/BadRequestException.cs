using System.Net;

namespace Application.Common.Exceptions;

public sealed class BadRequestException : AbstractHttpException
{
    public BadRequestException(string title, object? details = null)
        : base(HttpStatusCode.BadRequest, title, details)
    {
    }
}