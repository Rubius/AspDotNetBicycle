using Common.Models.Users;
using Common.Services;

namespace WebApp.Services.Auth;

public class PermissionService : IPermissionService
{
    public IEnumerable<Permission> GetPermissionsFromLdapGroups(IEnumerable<string?> memberOf)
    {
        var groups = memberOf.Where(x => !string.IsNullOrEmpty(x));
        
        return !groups.Any() 
            ? Enumerable.Empty<Permission>() 
            : Enum.GetValues<Permission>();
    }
}