using Common.Models.Auth;
using Common.Models.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Requests.Users.Services;

/// <summary>
/// Сервис контроля входа/выхода пользователя
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// Предоставить доступ пользователю к системе
    /// </summary>
    /// <param name="user">Данные пользователя</param>
    /// <param name="httpContext">http контекст</param>
    /// <returns>AuthResponce с токенами</returns>
    Task<AuthResult> Login(User user, HttpContext httpContext);

    /// <summary>
    /// Продлить доступ пользователя к системе
    /// </summary>
    /// <param name="oldToken">Прежний access токен</param>
    /// <param name="httpContext">http контекст</param>
    /// <returns>AuthResponce с токенами</returns>
    Task<AuthResult> Refresh(string oldToken, HttpContext httpContext);

    /// <summary>
    /// Разлогинить пользователя
    /// </summary>
    /// <param name="httpContext">http контекст</param>
    /// <returns>true – успех, false – не получилось разлогинить</returns>
    bool Logout(HttpContext httpContext);
}