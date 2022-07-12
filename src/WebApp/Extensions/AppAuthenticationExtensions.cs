using Application.Common.Extensions;
using Common.Models.Auth;
using Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebApp.Services;
using WebApp.Services.Auth;
using WebApp.Services.Mock;

namespace WebApp.Extensions;

public static class AppAuthenticationExtensions
{
    public static IServiceCollection AddAppAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));

        if (!configuration.IsTestEnvironment())
        {
            // TODO: раскомментировать для использования аутентификации через AD (по LDAP)
            // services.AddTransient<IAuthenticatorService, LdapAuthenticator>();
            services.AddTransient<IAuthenticatorService, MockAuthenticatorService>();

            // TODO: раскомментировать для использования получения пользователя из AD (по LDAP)
            // services.AddTransient<IUsersCrudService, LdapUsersCrudService>();
            services.AddTransient<IUsersCrudService, MockUsersCrudService>();
        }

        var jwtSection = configuration.GetSection("Auth:JwtSettings");
        services.AddSingleton(_ => new JwtSettings(
            jwtSection["issuer"],
            jwtSection["audience"],
            jwtSection["secretKey"],
            byte.Parse(jwtSection["accessTokenLifetimeMinutes"]),
            byte.Parse(jwtSection["refreshTokenLifetimeDays"])));

        var securitySection = configuration.GetSection("Auth:Security");
        services.AddSingleton(_ => new SecuritySettings(
            bool.Parse(securitySection["restrictParallelSessions"]),
            int.Parse(securitySection["parallelSessionMaxCount"])));

        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IPermissionService, PermissionService>();
        services.AddTransient<IAuditService, AuditService>();

        var serviceProvider = services.BuildServiceProvider();
        var jwtSettings = serviceProvider.GetRequiredService<JwtSettings>();

        services.AddAuthentication(x =>
            {
                x.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = !isDevelopment;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = !isDevelopment,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = jwtSettings.SecurityKey,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }

    public static IApplicationBuilder UseAppAuthentication(this IApplicationBuilder app)
    {
        app.UseJwtFromCookie();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}