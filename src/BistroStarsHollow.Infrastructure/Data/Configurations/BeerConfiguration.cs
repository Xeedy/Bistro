using BistroStarsHollow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BistroStarsHollow.Infrastructure.Data.Configurations;

public class BeerConfiguration : IEntityTypeConfiguration<Beer>
{
    public void Configure(EntityTypeBuilder<Beer> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Style).HasMaxLength(100);
        builder.Property(e => e.Capacity).HasMaxLength(20);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.ImagePath).HasMaxLength(500);
        builder.Property(e => e.Price).HasColumnType("decimal(10,2)");
        builder.Property(e => e.PriceSmall).HasColumnType("decimal(10,2)");

        builder.HasOne(e => e.Brewery)
            .WithMany(b => b.Beers)
            .HasForeignKey(e => e.BreweryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.Type);
    }
}
