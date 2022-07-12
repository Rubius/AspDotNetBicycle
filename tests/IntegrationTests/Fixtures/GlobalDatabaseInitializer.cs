using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Fixtures;

public static class GlobalDatabaseInitializer
{
    // thread safety is not a problem because of single threading xunit runner configuration
    private static bool _isInitialized;
        
    public static async Task InitializeIfNeeded(ApplicationDbContext dbContext)
    {
        if (_isInitialized)
        {
            return;
        }
            
        await dbContext.Database.MigrateAsync();
        _isInitialized = true;
    }
}