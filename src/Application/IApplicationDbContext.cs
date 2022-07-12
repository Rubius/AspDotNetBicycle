using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface IApplicationDbContext : IDisposable
{
    public DbSet<BicycleModel> BicycleModels { get; set; }

    public DbSet<Bicycle> Bicycles { get; set; }
    public DbSet<Ride> Rides { get; set; }

    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    int SaveChanges();
}