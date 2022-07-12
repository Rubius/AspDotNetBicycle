using System.Net;

namespace Application.Common.Exceptions;

public sealed class NotAllowedException : AbstractHttpException
{
    public NotAllowedException(string title, object? details = null)
        : base(HttpStatusCode.MethodNotAllowed, title, details)
    {
    }
}