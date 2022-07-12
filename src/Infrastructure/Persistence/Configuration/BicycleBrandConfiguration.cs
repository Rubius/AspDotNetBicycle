using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class BicycleBrandConfiguration : IEntityTypeConfiguration<BicycleBrand>
{
    public void Configure(EntityTypeBuilder<BicycleBrand> builder)
    {
        builder.OwnsOne(x => x.ManufacturerAddress, navigationBuilder =>
        {
            navigationBuilder.Property(address => address.Country)
                                .HasColumnName("ManufacturerCountry");
            navigationBuilder.Property(address => address.Region)
                                .HasColumnName("ManufacturerRegion");
            navigationBuilder.Property(address => address.City)
                                .HasColumnName("ManufacturerCity");
            navigationBuilder.Property(address => address.Street)
                                .HasColumnName("ManufacturerStreet");
        });
    }
}