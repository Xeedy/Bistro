using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using BistroStarsHollow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BistroStarsHollow.Infrastructure.Services;

public class ReservationService : IReservationService
{
    private static readonly TimeSpan ReservationDuration = TimeSpan.FromHours(2);
    private const int MaxActiveReservationsPerPhone = 3;
    private static readonly TimeZoneInfo CzechTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague");

    private readonly ApplicationDbContext _dbContext;

    public ReservationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TableAvailabilityDto>> GetTableAvailabilityAsync(DateTime date, TimeSpan time, int numberOfPeople)
    {
        var tables = await _dbContext.BistroTables
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var result = new List<TableAvailabilityDto>();

        foreach (var table in tables)
        {
            var isAvailable = table.Capacity >= numberOfPeople
                && await IsTableAvailableAsync(table.Id, date, time);

            result.Add(new TableAvailabilityDto
            {
                Id = table.Id,
                Name = table.Name,
                Capacity = table.Capacity,
                SortOrder = table.SortOrder,
                IsAvailable = isAvailable
            });
        }

        return result;
    }

    public async Task<(bool Success, ReservationResultDto? Result, string? Error)> CreateReservationAsync(
        ReservationCreateDto dto, string? ipAddress)
    {
        // Validate opening hours
        if (!await IsWithinOpeningHoursAsync(dto.Date, dto.Time))
            return (false, null, "Bistro je v požadovaném termínu zavřené.");

        // Validate not in the past
        var nowCzech = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CzechTimeZone);
        var requestedDateTime = dto.Date.Date + dto.Time;
        if (requestedDateTime <= nowCzech)
            return (false, null, "Nelze vytvořit rezervaci v minulosti.");

        // Validate table exists and has sufficient capacity
        var table = await _dbContext.BistroTables
            .FirstOrDefaultAsync(t => t.Id == dto.TableId && t.IsActive);

        if (table == null)
            return (false, null, "Vybraný stůl neexistuje.");

        if (table.Capacity < dto.NumberOfPeople)
            return (false, null, $"Stůl {table.Name} má kapacitu pouze {table.Capacity} osob.");

        // Validate table availability (race condition protection - re-check before insert)
        if (!await IsTableAvailableAsync(dto.TableId, dto.Date, dto.Time))
            return (false, null, "Vybraný stůl je v požadovaném čase již obsazený.");

        // Validate max reservations per phone
        var activeCount = await GetActiveReservationCountByPhoneAsync(dto.Phone);
        if (activeCount >= MaxActiveReservationsPerPhone)
            return (false, null, $"Na telefonní číslo {dto.Phone} již existují {MaxActiveReservationsPerPhone} aktivní rezervace.");

        var reservation = new Reservation
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Date = dto.Date.Date,
            Time = dto.Time,
            NumberOfPeople = dto.NumberOfPeople,
            TableId = dto.TableId,
            Note = dto.Note,
            IpAddress = ipAddress,
            Status = ReservationStatus.PendingConfirmation
        };

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        var result = new ReservationResultDto
        {
            ReservationId = reservation.Id,
            CancelToken = reservation.CancelToken,
            FirstName = reservation.FirstName,
            LastName = reservation.LastName,
            Date = reservation.Date,
            Time = reservation.Time,
            NumberOfPeople = reservation.NumberOfPeople,
            TableName = table.Name
        };

        return (true, result, null);
    }

    public async Task<(bool Success, string? Error)> CancelReservationAsync(string cancelToken)
    {
        var reservation = await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.CancelToken == cancelToken);

        if (reservation == null)
            return (false, "Rezervace nebyla nalezena.");

        if (reservation.Status == ReservationStatus.CancelledByUser
            || reservation.Status == ReservationStatus.CancelledByAdmin)
            return (false, "Tato rezervace již byla zrušena.");

        if (reservation.Status == ReservationStatus.Completed
            || reservation.Status == ReservationStatus.NoShow)
            return (false, "Tuto rezervaci již nelze zrušit.");

        reservation.Status = ReservationStatus.CancelledByUser;
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<bool> IsWithinOpeningHoursAsync(DateTime date, TimeSpan time)
    {
        var dayOfWeekCz = MapToDayOfWeekCz(date.DayOfWeek);

        var hours = await _dbContext.OpeningHours
            .FirstOrDefaultAsync(o => o.DayOfWeek == dayOfWeekCz);

        if (hours == null || hours.IsClosed || hours.OpenTime == null || hours.CloseTime == null)
            return false;

        // Must be within opening hours and start at least 30 min before closing
        var latestStart = hours.CloseTime.Value - TimeSpan.FromMinutes(30);

        return time >= hours.OpenTime.Value && time <= latestStart;
    }

    public async Task<bool> IsTableAvailableAsync(Guid tableId, DateTime date, TimeSpan time)
    {
        var requestedStart = time;
        var requestedEnd = time + ReservationDuration;

        var hasCollision = await _dbContext.Reservations
            .AnyAsync(r => r.TableId == tableId
                && r.Date == date.Date
                && (r.Status == ReservationStatus.PendingConfirmation || r.Status == ReservationStatus.Confirmed)
                && r.Time < requestedEnd
                && r.Time + ReservationDuration > requestedStart);

        return !hasCollision;
    }

    public async Task<int> GetActiveReservationCountByPhoneAsync(string phone)
    {
        var normalizedPhone = phone.Replace(" ", "");
        var nowCzech = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CzechTimeZone);
        var today = nowCzech.Date;

        return await _dbContext.Reservations
            .CountAsync(r => r.Phone.Replace(" ", "") == normalizedPhone
                && (r.Status == ReservationStatus.PendingConfirmation || r.Status == ReservationStatus.Confirmed)
                && (r.Date > today || (r.Date == today && r.Time + ReservationDuration > nowCzech.TimeOfDay)));
    }

    private static DayOfWeekCz MapToDayOfWeekCz(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => DayOfWeekCz.Monday,
            DayOfWeek.Tuesday => DayOfWeekCz.Tuesday,
            DayOfWeek.Wednesday => DayOfWeekCz.Wednesday,
            DayOfWeek.Thursday => DayOfWeekCz.Thursday,
            DayOfWeek.Friday => DayOfWeekCz.Friday,
            DayOfWeek.Saturday => DayOfWeekCz.Saturday,
            DayOfWeek.Sunday => DayOfWeekCz.Sunday,
            _ => DayOfWeekCz.Monday
        };
    }
}
