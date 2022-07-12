using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Common.Models.Users;

namespace Common.Extensions;

/// <summary>
/// Расширения для <see cref="HttpContext" />
/// </summary>
public static class HttpContextExtensions
{
    public const string JWT_COOKIE_KEY = ".myProject.jwtToken";

    /// <summary>
    /// Добавить jwt токен в http-only куки
    /// </summary>
    /// <param name="httpContext">контекст http</param>
    /// <param name="token">jwt токен</param>
    /// <param name="lifeTime">длительность жизни токена</param>
    public static void AddJwtToResponseCookies(this HttpContext httpContext, string token, TimeSpan lifeTime)
    {
        httpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
        httpContext.Response.Cookies.Append(JWT_COOKIE_KEY, token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                MaxAge = lifeTime,
                Path = "/; SameSite=None"
            });
    }

    /// <summary>
    /// Получить jwt токен из http-only куки
    /// </summary>
    /// <param name="httpContext">контекст http</param>
    /// <returns>токен</returns>
    public static string GetJwtFromRequestCookies(this HttpContext httpContext)
    {
        return httpContext.Request.Cookies[JWT_COOKIE_KEY];
    }

    /// <summary>
    /// Удалить jwt из http-only куки
    /// </summary>
    /// <param name="httpContext">контекст http</param>
    public static void DeleteJwtFromResponseCookies(this HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(JWT_COOKIE_KEY);
    }

    /// <summary>
    /// Получить Id пользователя
    /// </summary>
    /// <param name="httpContext">контекст http</param>
    /// <returns>Guid пользователя</returns>
    public static Guid GetUserId(this HttpContext httpContext)
    {
        var value = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return !string.IsNullOrEmpty(value)
            ? Guid.Parse(value)
            : Guid.Empty;
    }

    /// <summary>
    /// Получить типы доступа пользователя 
    /// </summary>
    /// <param name="httpContext">контекст http</param>
    /// <returns>Список типов досутпа пользователя</returns>
    public static IEnumerable<Permission> GetUserPermissions(this HttpContext httpContext)
    {
        return httpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.UserData)
            .SelectMany(x => x.Value.Split("|"))
            .Select(Enum.Parse<Permission>);
    }
}