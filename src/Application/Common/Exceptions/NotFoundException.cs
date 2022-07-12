using System.Net;

namespace Application.Common.Exceptions;

public sealed class NotFoundException : AbstractHttpException
{
    public NotFoundException(string title, string? resourceId = null) : 
        base(HttpStatusCode.NotFound, title, !string.IsNullOrEmpty(resourceId) ? new { Id = resourceId } : null)
    {
    }
}