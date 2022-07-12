using System.Text;
using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Extensions;
using Infrastructure.Persistence;
using Serilog;

namespace Infrastructure;

public static class InfrastructureInitializer
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isGenerationBuild)
    {
        InitializeDb(services, configuration, isGenerationBuild);
        return services;
    }

    private static void InitializeDb(
        IServiceCollection services,
        IConfiguration configuration,
        bool isGenerationBuild)
    {
        var dbConnectionString = GetDbConnectionString(configuration, isGenerationBuild);
        try
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var currentAssemblyName = typeof(ApplicationDbContext).Assembly.FullName;
                options.UseSqlServer(
                    dbConnectionString,
                    b => b.MigrationsAssembly(currentAssemblyName));
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Adding database to WebApp\n" +
                                 $"\t{dbConnectionString}");
            throw;
        }
    }


    private static string GetDbConnectionString(IConfiguration configuration, bool isGenerationBuild)
    {
        var useTestDb = configuration.IsTestEnvironment();
        var sectionName = useTestDb ? "db_test" : "db";
        var section = configuration.GetSection(sectionName);

        // Server=db,myPortNumber;Database=master;User=sa;Password=Your_password123;"

        var dbServer = section["Server"];
        var dbDatabase = section["Database"];
        var dbUsername = section["User"];
        var dbPassword = section["Password"];
        var dbPort = section["Port"];

        var connectionStringBuilder = new StringBuilder();
        connectionStringBuilder.Append($"Server={dbServer}");

        if (!string.IsNullOrWhiteSpace(dbPort))
        {
            connectionStringBuilder.Append($",{dbPort}");
        }
        connectionStringBuilder.Append(';');


        connectionStringBuilder.Append($"Database={dbDatabase};");
        connectionStringBuilder.Append($"User={dbUsername};");
        connectionStringBuilder.Append($"Password={dbPassword};");
        if (isGenerationBuild || useTestDb)
            connectionStringBuilder.Append("TrustServerCertificate=true;");

        return connectionStringBuilder.ToString();
    }
}
