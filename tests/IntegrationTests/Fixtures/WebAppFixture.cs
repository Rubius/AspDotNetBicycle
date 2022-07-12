using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests.Fixtures;

public class WebAppFixture : IDisposable
{
    public ApiWebAppFactory Factory { get; }

    public WebAppFixture()
    {
        Factory = new ApiWebAppFactory();
    }

    public async Task ResetDbState(Table[]? tablesToIgnore = null)
    {
        using var scope = Factory.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (dbContext is null)
        {
            throw new InvalidOperationException($"Can`t GetService {nameof(ApplicationDbContext)}");
        }
        await GlobalDatabaseInitializer.InitializeIfNeeded(dbContext);

        await using var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();

        var checkpoint = CreateCheckpoint(tablesToIgnore);
        await checkpoint.Reset(connection);
    }

    private static Checkpoint CreateCheckpoint(Table[]? tablesToIgnore)
    {
        var tablesToIgnoreList = new List<Table> { "__EFMigrationsHistory" };
        if (tablesToIgnore != null)
        {
            tablesToIgnoreList.AddRange(tablesToIgnore);
        }

        return new Checkpoint
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = tablesToIgnoreList.ToArray(),
        };
    }

    public void Dispose()
    {
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }
}