using Common.Models.Users;
using Common.Services;
using System.Security.Claims;
using Common.Extensions;

namespace WebApp.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User? User
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            var claims = user.Claims.ToList();
            var userName = claims.First(x => x.Type == ClaimTypes.Name).Value;
            var login = claims.First(x => x.Type == ClaimTypes.WindowsAccountName).Value;
            var userIdRaw = claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userId = Guid.Parse(userIdRaw);

            var permissions = httpContext!
                .GetUserPermissions()
                .ToList();

            return new User
            {
                Id = userId,
                Login = login,
                Name = userName,
                Permissions = permissions
            };
        }
    }
}