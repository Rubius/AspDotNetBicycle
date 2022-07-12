using Common.Extensions;
using Serilog;

namespace WebApp.Middlewares;

public class JwtFromCookieMiddleware
{
    private const string AUTHORIZATION_KEY = "Authorization";

    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;


    public JwtFromCookieMiddleware(
        RequestDelegate next)
    {
        _next = next;
        _logger = Log.ForContext<JwtFromCookieMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.GetJwtFromRequestCookies();

        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Append(AUTHORIZATION_KEY, $"Bearer {token}");
        }
        else if (context.Request.Headers.ContainsKey(AUTHORIZATION_KEY))
        {
            _logger.Warning(
                $"Jwt token is located in Authorization header: ({context.Request.Headers[AUTHORIZATION_KEY]}), it will be removed");
            context.Request.Headers.Remove(AUTHORIZATION_KEY);
        }

        await _next(context);
    }
}