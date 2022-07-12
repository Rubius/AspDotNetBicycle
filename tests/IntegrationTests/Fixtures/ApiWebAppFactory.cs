using Common.Services;
using IntegrationTests.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Fixtures;

public class ApiWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("IsTestEnvironment", true.ToString());

        builder.ConfigureServices(x =>
        {
            x.AddSingleton<IUsersCrudService, TestUsersCrudService>();
            x.AddSingleton<IAuthenticatorService, TestAuthenticatorService>();
        });
    }
}