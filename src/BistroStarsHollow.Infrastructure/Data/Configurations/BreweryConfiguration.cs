using BistroStarsHollow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BistroStarsHollow.Infrastructure.Data.Configurations;

public class BreweryConfiguration : IEntityTypeConfiguration<Brewery>
{
    public void Configure(EntityTypeBuilder<Brewery> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.LogoImagePath).HasMaxLength(500);
        builder.HasIndex(e => e.IsActive);
    }
}
