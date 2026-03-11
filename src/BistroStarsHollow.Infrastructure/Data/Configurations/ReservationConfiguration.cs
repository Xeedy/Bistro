using BistroStarsHollow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BistroStarsHollow.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(20).IsRequired();
        builder.Property(e => e.CancelToken).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.Property(e => e.IpAddress).HasMaxLength(45);

        builder.HasOne(e => e.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(e => e.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CancelToken).IsUnique();
    }
}
