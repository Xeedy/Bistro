using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using BistroStarsHollow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BistroStarsHollow.Infrastructure.Services;

public class AdminReservationService : IAdminReservationService
{
    private static readonly TimeZoneInfo CzechTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague");

    private readonly ApplicationDbContext _dbContext;

    public AdminReservationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ReservationListItemDto>> GetReservationsAsync(ReservationFilterDto filter)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Table)
            .AsQueryable();

        if (filter.Date.HasValue)
            query = query.Where(r => r.Date == filter.Date.Value.Date);

        if (filter.TableId.HasValue)
            query = query.Where(r => r.TableId == filter.TableId.Value);

        if (filter.Status.HasValue)
            query = query.Where(r => r.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            var search = filter.SearchPhrase.Trim().ToLower();
            query = query.Where(r =>
                (r.FirstName + " " + r.LastName).ToLower().Contains(search) ||
                r.Phone.Contains(search));
        }

        return await query
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.Time)
            .Select(r => new ReservationListItemDto
            {
                Id = r.Id,
                FullName = r.FirstName + " " + r.LastName,
                Phone = r.Phone,
                Date = r.Date,
                Time = r.Time,
                NumberOfPeople = r.NumberOfPeople,
                TableName = r.Table.Name,
                Status = r.Status,
                Note = r.Note,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(Guid id)
    {
        return await _dbContext.Reservations
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<BistroTable>> GetAllTablesAsync()
    {
        return await _dbContext.BistroTables
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }

    public async Task<int> GetPendingReservationCountAsync()
    {
        return await _dbContext.Reservations
            .CountAsync(r => r.Status == ReservationStatus.PendingConfirmation);
    }

    public async Task<(bool Success, string? Error)> ConfirmReservationAsync(Guid id)
    {
        return await TransitionStatusAsync(id, ReservationStatus.Confirmed,
            new[] { ReservationStatus.PendingConfirmation });
    }

    public async Task<(bool Success, string? Error)> CancelReservationByAdminAsync(Guid id)
    {
        return await TransitionStatusAsync(id, ReservationStatus.CancelledByAdmin,
            new[] { ReservationStatus.PendingConfirmation, ReservationStatus.Confirmed });
    }

    public async Task<(bool Success, string? Error)> MarkAsCompletedAsync(Guid id)
    {
        return await TransitionStatusAsync(id, ReservationStatus.Completed,
            new[] { ReservationStatus.Confirmed });
    }

    public async Task<(bool Success, string? Error)> MarkAsNoShowAsync(Guid id)
    {
        return await TransitionStatusAsync(id, ReservationStatus.NoShow,
            new[] { ReservationStatus.Confirmed });
    }

    public async Task<int> GetTodayReservationCountAsync()
    {
        var todayCzech = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CzechTimeZone).Date;
        return await _dbContext.Reservations
            .CountAsync(r => r.Date == todayCzech &&
                (r.Status == ReservationStatus.PendingConfirmation || r.Status == ReservationStatus.Confirmed));
    }

    public async Task<List<ReservationListItemDto>> GetUpcomingReservationsAsync(int count)
    {
        var nowCzech = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CzechTimeZone);
        var today = nowCzech.Date;

        return await _dbContext.Reservations
            .Include(r => r.Table)
            .Where(r => (r.Date > today || (r.Date == today && r.Time >= nowCzech.TimeOfDay)) &&
                (r.Status == ReservationStatus.PendingConfirmation || r.Status == ReservationStatus.Confirmed))
            .OrderBy(r => r.Date)
            .ThenBy(r => r.Time)
            .Take(count)
            .Select(r => new ReservationListItemDto
            {
                Id = r.Id,
                FullName = r.FirstName + " " + r.LastName,
                Phone = r.Phone,
                Date = r.Date,
                Time = r.Time,
                NumberOfPeople = r.NumberOfPeople,
                TableName = r.Table.Name,
                Status = r.Status,
                Note = r.Note,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<(bool Success, string? Error)> TransitionStatusAsync(
        Guid id, ReservationStatus newStatus, ReservationStatus[] allowedFromStatuses)
    {
        var reservation = await _dbContext.Reservations.FindAsync(id);

        if (reservation == null)
            return (false, "Rezervace nebyla nalezena.");

        if (!allowedFromStatuses.Contains(reservation.Status))
            return (false, $"Rezervaci ve stavu '{GetStatusLabel(reservation.Status)}' nelze převést na '{GetStatusLabel(newStatus)}'.");

        reservation.Status = newStatus;
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }

    private static string GetStatusLabel(ReservationStatus status) => status switch
    {
        ReservationStatus.PendingConfirmation => "Čeká na potvrzení",
        ReservationStatus.Confirmed => "Potvrzena",
        ReservationStatus.CancelledByUser => "Zrušena hostem",
        ReservationStatus.CancelledByAdmin => "Zrušena adminem",
        ReservationStatus.Completed => "Dokončena",
        ReservationStatus.NoShow => "Nedostavení",
        _ => status.ToString()
    };
}
