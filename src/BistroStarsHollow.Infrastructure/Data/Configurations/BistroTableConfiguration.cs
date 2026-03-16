using BistroStarsHollow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BistroStarsHollow.Infrastructure.Data.Configurations;

public class BistroTableConfiguration : IEntityTypeConfiguration<BistroTable>
{
    public void Configure(EntityTypeBuilder<BistroTable> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Zone).HasMaxLength(50);
        builder.HasIndex(e => e.IsActive);
    }
}
