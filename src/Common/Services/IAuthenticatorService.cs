using Common.Models.Auth;

namespace Common.Services;
/// <summary>
/// Сервис аутентификации пользователя
/// </summary>
public interface IAuthenticatorService
{
    /// <summary>
    /// Провести аутентификацию пользователя
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Данные о пользователе</returns>
    Task<ExternalAuthUser?> Authenticate(string login, string password);
}
