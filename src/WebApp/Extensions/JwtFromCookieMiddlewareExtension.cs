using WebApp.Middlewares;

namespace WebApp.Extensions;

public static class JwtFromCookieMiddlewareExtension
{
    public static IApplicationBuilder UseJwtFromCookie(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtFromCookieMiddleware>();
    }
}