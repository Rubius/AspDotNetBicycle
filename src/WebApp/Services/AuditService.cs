using Common.Services;
using Serilog;

namespace WebApp.Services;

public class AuditService : IAuditService
{
    private readonly ICurrentUserService _currentUserService;

    public AuditService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
        
    public void LogAudit(string action, string comment, object? metadata)
    {
        var user = _currentUserService.User;
        var userName = user?.Name != null 
            ? $"; User Name = {user.Name}" : 
            "";

        var userPermissions = user != null 
            ? $"; User Permissions = {string.Join(';', user.Permissions.Select(x => x.ToString()))}" 
            : "";

        var metadataString = metadata?.ToString() != null
            ? $"; Data: {metadata}"
            : "";
        var commentString = !string.IsNullOrEmpty(comment)
            ? $"; Comment = {comment}"
            : "";

        Log.Information("[AUDIT] {Action}{UserName}{UserRole}{Comment}{Metadata}",
            action, userName, userPermissions, commentString, metadataString);
    }
}