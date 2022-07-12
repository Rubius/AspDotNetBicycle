using Application;
using Common.Models.Users;
using Common.Services;
using Domain.Entities;
using IntegrationTests.Fixtures;
using IntegrationTests.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Common.Extensions;

namespace IntegrationTests;

public class ApiClientManager
{
    public BicycleApiClient Bicycles { get; }
    public BicycleModelsApiClient BicycleModels { get; }
    public UsersApiClient Users { get; }

    public User TestUser { get; }

    public ApiClientManager(ApiWebAppFactory appFactory, bool withoutCookies)
    {
        var usersRetriever = (TestUsersCrudService)appFactory.Services.GetRequiredService<IUsersCrudService>();
        TestUser = usersRetriever.User;

        var baseUrl = appFactory.Server.BaseAddress.ToString();
        var httpClient = appFactory.CreateClient();

        if (!withoutCookies)
        {
            var (cookie, refreshToken) = GetJwtCookie(appFactory.Services);
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            AddUserDataInDb(appFactory.Services, refreshToken, baseUrl);
        }

        Bicycles = new BicycleApiClient(httpClient) { BaseUrl = baseUrl };
        BicycleModels = new BicycleModelsApiClient(httpClient) { BaseUrl = baseUrl };
        Users = new UsersApiClient(httpClient) { BaseUrl = baseUrl };
    }

    private (string cookies,  string refreshToken) GetJwtCookie(IServiceProvider serviceProvider)
    {
        var jwtTokenManager = serviceProvider.GetRequiredService<IJwtTokenService>();
        var token = jwtTokenManager.GenerateJwtToken(TestUser);
        var refreshToken = jwtTokenManager.GenerateRefreshToken();

        var cookieBody = $"{HttpContextExtensions.JWT_COOKIE_KEY}={token}; Path=/; Domain=localhost; Secure; HttpOnly; Expires=Fri, 02 Apr 2028 05:16:52 GMT;";

        return (cookieBody, refreshToken);
    }

    private void AddUserDataInDb(IServiceProvider serviceProvider, string refreshToken, string baseUrl)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        if (dbContext is null)
        {
            throw new Exception("Can`t init dbContext");
        }

        dbContext.UserRefreshTokens.Add(new UserRefreshToken(TestUser.Id, refreshToken, TimeSpan.FromDays(100), baseUrl));
        dbContext.SaveChanges();
    }
}