using Application.Common.Exceptions;
using Common.Extensions;
using Common.Models.Auth;
using Common.Models.Users;
using Common.Services;
using Domain.Entities;
using Localization.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Users.Services;

/// <summary>
/// Сервис авторизации пользователя
/// </summary>
public class LoginService : ILoginService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly SecuritySettings _securitySettings;
    private readonly IAuditService _auditService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUsersCrudService _usersCrudService;

    public LoginService(
        IAuditService auditService,
        IJwtTokenService jwtTokenService,
        JwtSettings jwtSettings,
        IApplicationDbContext dbContext,
        IUsersCrudService usersCrudService,
        SecuritySettings securitySettings)
    {
        _auditService = auditService;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
        _dbContext = dbContext;
        _usersCrudService = usersCrudService;
        _securitySettings = securitySettings;
    }

    public async Task<AuthResult> Login(User user, HttpContext httpContext)
    {
        _auditService.LogAudit("Login", string.Empty, new
        {
            user.Name,
            user.Permissions
        });
        await DeleteExtraTokensIfNeed(user.Id);

        return await CreateTokensPair(user, httpContext);
    }

    /// <summary>
    /// Удалить токены пользователя, если их количество больше допустимого <see cref="SecuritySettings.ParallelSessionMaxCount"/>
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    private async Task DeleteExtraTokensIfNeed(Guid userId)
    {
        var query = _dbContext.UserRefreshTokens.AsQueryable();
        var userTokensQuery = UserRefreshToken.UserTokens(query, userId);

        if (_securitySettings.RestrictParallelSessions)
        {
            await DeleteTokens(userTokensQuery);
        }
        else
        {
            int tokensCount = await userTokensQuery.CountAsync();
            int extraTokensCount = tokensCount - _securitySettings.ParallelSessionMaxCount + 1;
            if (extraTokensCount > 0)
            {
                await DeleteTokens(UserRefreshToken.UserExtraTokens(query, userId, extraTokensCount));
            }
        }
    }

    private async Task DeleteTokens(IQueryable<UserRefreshToken> query)
    {
        var entities = await query.ToListAsync();
        _dbContext.UserRefreshTokens.RemoveRange(entities);
    }

    public async Task<AuthResult> Refresh(string oldToken, HttpContext httpContext)
    {
        var userHost = httpContext.Request.Host.Value;

        var oldTokenEntity = await GetUserRefreshToken(oldToken);
        VerifyUserData(userHost, oldTokenEntity);
        ValidateTokenExpiredAt(oldTokenEntity);

        var user = await _usersCrudService.GetByIdAsync(oldTokenEntity.UserId);
        if (user is null)
        {
            throw new BadRequestException(Resources.UserNotFound, new { oldTokenEntity.UserId });
        }

        _auditService.LogAudit("Refresh", string.Empty, new
        {
            user.Name,
            user.Permissions
        });

        return await CreateTokensPair(user, httpContext);
    }

    public bool Logout(HttpContext httpContext)
    {
        _auditService.LogAudit("Logout", string.Empty);

        httpContext.DeleteJwtFromResponseCookies();
        var cookie = httpContext.GetJwtFromRequestCookies();

        return string.IsNullOrEmpty(cookie);
    }

    /// <summary>
    /// Сформировать пару токенов для пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private async Task<AuthResult> CreateTokensPair(User user, HttpContext httpContext)
    {
        var accessToken = _jwtTokenService.GenerateJwtToken(user);
        httpContext.AddJwtToResponseCookies(accessToken, _jwtSettings.AccessTokenLifetimeMinutes);

        var refreshToken = UpdateRefreshToken(user.Id, httpContext.Request.Host.Value);

        await _dbContext.SaveChangesAsync();

        return new AuthResult(accessToken, refreshToken, _jwtSettings.AccessTokenLifetimeMinutes, user);
    }

    /// <summary>
    /// Добавить новый refresh токен для пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="ipAddress">Хост пользователя</param>
    /// <returns>refresh токен</returns>
    private string UpdateRefreshToken(Guid userId, string ipAddress)
    {
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        _dbContext.UserRefreshTokens.Add(new UserRefreshToken(userId, refreshToken, _jwtSettings.RefreshTokenLifetimeDays, ipAddress));

        return refreshToken;
    }

    /// <summary>
    /// Получить данные refresh токена пользователя из БД
    /// </summary>
    /// <param name="oldRefreshToken">Строковое представление refresh токена</param>
    /// <returns>Данные о refresh токене</returns>
    private async Task<UserRefreshToken> GetUserRefreshToken(string oldRefreshToken)
    {
        var oldTokenEntity = await FindRefreshToken(oldRefreshToken);
        _dbContext.UserRefreshTokens.Remove(oldTokenEntity);

        return oldTokenEntity;
    }

    /// <summary>
    /// Найти данные refresh токена в БД
    /// </summary>
    /// <param name="oldRefreshToken">СТроковое представление токена</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Пользователь не авторизован, если токен не обнаружен</exception>
    private async Task<UserRefreshToken> FindRefreshToken(string? oldRefreshToken)
    {
        var oldTokenEntity = await _dbContext.UserRefreshTokens
            .Where(x => x.RefreshToken == oldRefreshToken)
            .FirstOrDefaultAsync();
        if (oldTokenEntity is null)
        {
            throw new UnauthorizedException(Resources.UserIsUnauthorized);
        }

        return oldTokenEntity;
    }

    /// <summary>
    /// Проверить совпадение данных о refresh токене в БД и данных текущего запроса
    /// </summary>
    /// <param name="ipAddress">Хост пользователя</param>
    /// <param name="oldTokenEntity">Данные refresh токена</param>
    /// <exception cref="UnauthorizedException">Пользователь не авторизован, если данные не совпадают</exception>
    private static void VerifyUserData(string ipAddress, UserRefreshToken oldTokenEntity)
    {
        if (!oldTokenEntity.IsSameIpAddress(ipAddress))
        {
            throw new UnauthorizedException(Resources.UserIsUnauthorized);
        }
    }

    /// <summary>
    /// Проверить время жизни refresh токена
    /// </summary>
    /// <param name="oldTokenEntity">Данные refresh токена</param>
    /// <exception cref="UnauthorizedException">Пользователь не авторизован, если токен протух</exception>
    private static void ValidateTokenExpiredAt(UserRefreshToken oldTokenEntity)
    {
        if (oldTokenEntity.TokenIsExpired)
        {
            throw new UnauthorizedException(Resources.UserIsUnauthorized);
        }
    }
}