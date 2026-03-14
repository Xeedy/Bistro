using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using BistroStarsHollow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BistroStarsHollow.Infrastructure.Services;

public class ShiftService : IShiftService
{
    private readonly ApplicationDbContext _context;

    public ShiftService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shift>> GetShiftsForMonthAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        return await _context.Shifts
            .Include(s => s.Assignments)
            .Where(s => s.Date >= startDate && s.Date < endDate)
            .OrderBy(s => s.Date)
            .ToListAsync();
    }

    public async Task<Shift?> GetShiftByDateAsync(DateTime date)
    {
        return await _context.Shifts
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Date == date.Date);
    }

    public async Task<Shift?> GetShiftByIdAsync(Guid id)
    {
        return await _context.Shifts
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shift> GetOrCreateShiftAsync(DateTime date)
    {
        var shift = await GetShiftByDateAsync(date);
        if (shift != null) return shift;

        shift = new Shift
        {
            Date = date.Date,
            Type = ShiftType.FullDay,
            RequiredStaffCount = 1,
            DayStatus = DayStatus.Open
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        return shift;
    }

    public async Task UpdateShiftSettingsAsync(Guid shiftId, ShiftType type, int requiredStaffCount, DayStatus dayStatus, string? note)
    {
        var shift = await _context.Shifts.FindAsync(shiftId);
        if (shift == null) return;

        shift.Type = type;
        shift.RequiredStaffCount = requiredStaffCount;
        shift.DayStatus = dayStatus;
        shift.Note = note;

        await _context.SaveChangesAsync();
    }

    public async Task<(bool Success, string? Error)> SignUpAsync(DateTime date, string userId, string userDisplayName)
    {
        if (date.Date < DateTime.Today)
            return (false, "Nelze se zapsat na směnu v minulosti.");

        var shift = await GetOrCreateShiftAsync(date);

        if (shift.DayStatus != DayStatus.Open)
            return (false, "Tento den je označen jako zavřený.");

        if (shift.Assignments.Any(a => a.UserId == userId))
            return (false, "Na tuto směnu jste již zapsán/a.");

        if (shift.Assignments.Count >= shift.RequiredStaffCount)
            return (false, $"Směna je již plně obsazena ({shift.RequiredStaffCount} míst).");

        var assignment = new ShiftAssignment
        {
            ShiftId = shift.Id,
            UserId = userId,
            UserDisplayName = userDisplayName
        };

        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> SignOffAsync(DateTime date, string userId)
    {
        if (date.Date < DateTime.Today)
            return (false, "Nelze se odepsat ze směny v minulosti.");

        var shift = await GetShiftByDateAsync(date);
        if (shift == null)
            return (false, "Pro tento den neexistuje směna.");

        var assignment = shift.Assignments.FirstOrDefault(a => a.UserId == userId);
        if (assignment == null)
            return (false, "Na tuto směnu nejste zapsán/a.");

        _context.ShiftAssignments.Remove(assignment);
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> OverrideAssignmentAsync(DateTime date, string newUserId, string newUserDisplayName, string existingUserId)
    {
        if (date.Date < DateTime.Today)
            return (false, "Nelze přepsat směnu v minulosti.");

        var shift = await GetShiftByDateAsync(date);
        if (shift == null)
            return (false, "Pro tento den neexistuje směna.");

        var existingAssignment = shift.Assignments.FirstOrDefault(a => a.UserId == existingUserId);
        if (existingAssignment == null)
            return (false, "Uživatel, kterého chcete přepsat, není na této směně zapsán.");

        if (shift.Assignments.Any(a => a.UserId == newUserId))
            return (false, "Na tuto směnu jste již zapsán/a.");

        existingAssignment.UserId = newUserId;
        existingAssignment.UserDisplayName = newUserDisplayName;
        existingAssignment.Note = $"Přepsáno (původně: {existingAssignment.UserDisplayName})";

        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<List<ShiftAssignment>> GetUpcomingAssignmentsForUserAsync(string userId, int count)
    {
        return await _context.ShiftAssignments
            .Include(a => a.Shift)
            .Where(a => a.UserId == userId && a.Shift.Date >= DateTime.Today)
            .OrderBy(a => a.Shift.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> IsUserOnShiftAsync(string userId, DateTime date)
    {
        return await _context.ShiftAssignments
            .AnyAsync(a => a.UserId == userId && a.Shift.Date == date.Date);
    }
}
