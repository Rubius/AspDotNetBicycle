using Common.Models.Users;

namespace Common.Services;

/// <summary>
/// Сервис формирования токенов доступа в систему
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Генерирует Jwt токен
    /// </summary>
    /// <param name="user">пользователь</param>
    /// <returns></returns>
    string GenerateJwtToken(User user);

    /// <summary>
    /// Генерирует Refresh токен для обновления токена доступа
    /// </summary>
    /// <returns>refresh токен</returns>
    string GenerateRefreshToken();
}