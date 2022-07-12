using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common.Models.Auth;

/// <summary>
/// Настройки Jwt
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Издатель токена
    /// </summary>
    public string Issuer { get; }

    /// <summary>
    /// Потребители токена
    /// </summary>
    public string Audience { get; }

    /// <summary>
    /// Секретный ключ
    /// </summary>
    public SymmetricSecurityKey SecurityKey { get; }

    /// <summary>
    /// Подпись токена
    /// </summary>
    public SigningCredentials SigningCredentials { get; }
        
    /// <summary>
    /// Время жизни access токена в минутах
    /// </summary>
    public TimeSpan AccessTokenLifetimeMinutes { get; }

    /// <summary>
    /// Время жизни refresh токена в часах
    /// </summary>
    public TimeSpan RefreshTokenLifetimeDays { get; }

    public JwtSettings(string issuer, string audience, string secretKey, byte accessTokenLifetimeminutes, byte refreshTokenLifetimeDays)
    {
        Issuer = issuer;
        Audience = audience;
        SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        AccessTokenLifetimeMinutes = TimeSpan.FromMinutes(accessTokenLifetimeminutes > 1
            ? accessTokenLifetimeminutes
            : 1);
        RefreshTokenLifetimeDays = TimeSpan.FromDays(refreshTokenLifetimeDays > 1
            ? refreshTokenLifetimeDays
            : 1);

    }
}