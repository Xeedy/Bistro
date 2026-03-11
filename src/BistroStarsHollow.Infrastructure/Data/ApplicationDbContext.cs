using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Common;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Infrastructure.Identity;

namespace BistroStarsHollow.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IDataProtectionKeyContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Beer> Beers => Set<Beer>();
    public DbSet<Brewery> Breweries => Set<Brewery>();
    public DbSet<BistroTable> BistroTables => Set<BistroTable>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<CashClosure> CashClosures => Set<CashClosure>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<HeroSlide> HeroSlides => Set<HeroSlide>();
    public DbSet<OpeningHours> OpeningHours => Set<OpeningHours>();
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
    public DbSet<StaffOnDuty> StaffOnDuty => Set<StaffOnDuty>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private void UpdateAuditFields()
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService?.UserId;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = userId;
                    break;
            }
        }
    }
}
