using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class UserRefreshToken
{
    public UserRefreshToken(Guid userId, string refreshToken, TimeSpan expiredAt, string requestedIpAddress)
    {
        UserId = userId;
        RefreshToken = refreshToken;
        ExpiredAt = CalculateExpiredAt(expiredAt);
        RequestedIpAddress = requestedIpAddress;
    }

#pragma warning disable CS8618
    protected UserRefreshToken() { }
#pragma warning restore CS8618

    /// <summary>
    /// Идентификатор
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Refresh токен
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Время окончания действия
    /// </summary>
    public DateTime ExpiredAt { get; set; }

    /// <summary>
    /// Хост запроса токена
    /// </summary>
    public string RequestedIpAddress { get; set; }

    /// <summary>
    /// Токен протух
    /// </summary>
    [NotMapped]
    public bool TokenIsExpired => ExpiredAt < DateTime.Now;

    /// <summary>
    /// Проверить совпадение ip адреса с текущим
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public bool IsSameIpAddress(string ipAddress) => string.Equals(RequestedIpAddress, ipAddress);
    private static DateTime CalculateExpiredAt(TimeSpan expiredAt) =>
        DateTime.Now.Add(expiredAt);

    /// <summary>
    /// Refresh токены пользователя
    /// </summary>
    /// <param name="tokensQuery">Запрос на refresh токены</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Отфильтрованный запрос по пользователю</returns>
    public static IQueryable<UserRefreshToken> UserTokens(IQueryable<UserRefreshToken> tokensQuery, Guid userId) =>
        tokensQuery.Where(x => x.UserId == userId);

    /// <summary>
    /// Лишние токены пользователя
    /// </summary>
    /// <param name="tokensQuery">Запрос на refresh токены</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="extraTokensCount">Количество лишних токенов</param>
    /// <returns>Запрос с лишними refresh токенами пользователя</returns>
    public static IQueryable<UserRefreshToken> UserExtraTokens(IQueryable<UserRefreshToken> tokensQuery, Guid userId, int extraTokensCount) =>
        UserTokens(tokensQuery, userId).OrderBy(x => x.ExpiredAt).Take(extraTokensCount);
}