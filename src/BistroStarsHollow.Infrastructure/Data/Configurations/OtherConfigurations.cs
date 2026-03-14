using BistroStarsHollow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BistroStarsHollow.Infrastructure.Data.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.HasIndex(e => e.Date).IsUnique();
        builder.HasMany(e => e.Assignments)
            .WithOne(a => a.Shift)
            .HasForeignKey(a => a.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ShiftAssignmentConfiguration : IEntityTypeConfiguration<ShiftAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftAssignment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.UserDisplayName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.HasIndex(e => new { e.ShiftId, e.UserId }).IsUnique();
    }
}

public class CashClosureConfiguration : IEntityTypeConfiguration<CashClosure>
{
    public void Configure(EntityTypeBuilder<CashClosure> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.Property(e => e.CardAmount).HasColumnType("decimal(10,2)");
        builder.Property(e => e.CashAmount).HasColumnType("decimal(10,2)");
        builder.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
        builder.Property(e => e.Difference).HasColumnType("decimal(10,2)");
        builder.HasIndex(e => e.Date);
    }
}

public class GalleryImageConfiguration : IEntityTypeConfiguration<GalleryImage>
{
    public void Configure(EntityTypeBuilder<GalleryImage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FileName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(e => e.ThumbnailPath).HasMaxLength(500);
        builder.Property(e => e.AltText).HasMaxLength(300);
        builder.HasIndex(e => e.IsActive);
    }
}

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.ImagePath).HasMaxLength(500);
        builder.HasIndex(e => e.IsActive);
    }
}

public class HeroSlideConfiguration : IEntityTypeConfiguration<HeroSlide>
{
    public void Configure(EntityTypeBuilder<HeroSlide> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ImagePath).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(200);
        builder.Property(e => e.LinkUrl).HasMaxLength(500);
        builder.HasIndex(e => e.IsActive);
    }
}

public class OpeningHoursConfiguration : IEntityTypeConfiguration<OpeningHours>
{
    public void Configure(EntityTypeBuilder<OpeningHours> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.DayOfWeek).IsUnique();
    }
}

public class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Key).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(200);
        builder.Property(e => e.Content).HasColumnType("nvarchar(max)");
        builder.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("cs");
        builder.HasIndex(e => new { e.Key, e.Language }).IsUnique();
    }
}

public class StaffOnDutyConfiguration : IEntityTypeConfiguration<StaffOnDuty>
{
    public void Configure(EntityTypeBuilder<StaffOnDuty> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasMaxLength(450).IsRequired();
        builder.Property(e => e.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.PhotoPath).HasMaxLength(500);
        builder.HasIndex(e => e.Date);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasMaxLength(450);
        builder.Property(e => e.UserName).HasMaxLength(200);
        builder.Property(e => e.Action).HasMaxLength(200).IsRequired();
        builder.Property(e => e.EntityType).HasMaxLength(200).IsRequired();
        builder.Property(e => e.EntityId).HasMaxLength(450);
        builder.Property(e => e.IpAddress).HasMaxLength(45);
        builder.Property(e => e.UserAgent).HasMaxLength(500);
        builder.Property(e => e.Details).HasColumnType("nvarchar(max)");
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.UserId);
    }
}
