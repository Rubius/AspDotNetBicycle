using Application.Common.Behaviours;
using Application.Requests.Users.Services;
using Common.Models.Auth;
using Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MappingBehaviour<,>));

        services.AddScoped<ILoginService>(e => new LoginService(
                e.GetRequiredService<IAuditService>(),
                e.GetRequiredService<IJwtTokenService>(),
                e.GetRequiredService<JwtSettings>(),
                e.GetRequiredService<IApplicationDbContext>(),
                e.GetRequiredService<IUsersCrudService>(),
                e.GetRequiredService<SecuritySettings>()));

        return services;
    }
}