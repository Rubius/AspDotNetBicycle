using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class BicycleConfiguration : IEntityTypeConfiguration<Bicycle>
{
    public void Configure(EntityTypeBuilder<Bicycle> builder)
    {
        builder.HasOne(x => x.Model)
            .WithMany(x => x.Bicycles)
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(x => x.RentalPointAddress, navigationBuilder =>
        {
            navigationBuilder.Property(address => address.Country)
                                .HasColumnName("RentalPointCountry");
            navigationBuilder.Property(address => address.Region)
                                .HasColumnName("RentalPointRegion");
            navigationBuilder.Property(address => address.City)
                                .HasColumnName("RentalPointCity");
            navigationBuilder.Property(address => address.Street)
                                .HasColumnName("RentalPointStreet");
        });
    }
}