using Application.Common.Exceptions;
using Common.Extensions;
using Common.Models.Users;
using Localization.Resources;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters;
/// <summary>
/// Проверка прав доступа по типам доступа пользователя
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthAttribute : Attribute, IAuthorizationFilter
{
    public AuthAttribute(Permission permission)
    {
        Permission = permission;
    }

    internal Permission Permission { get; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.GetUserPermissions().Contains(Permission))
        {
            throw new ForbiddenException(Resources.AccessDenied, Permission.ToString());
        }
    }
}
