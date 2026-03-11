using BistroStarsHollow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BistroStarsHollow.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.DisplayPrice).HasMaxLength(50);
        builder.Property(e => e.Price).HasColumnType("decimal(10,2)");
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsActive);
    }
}
