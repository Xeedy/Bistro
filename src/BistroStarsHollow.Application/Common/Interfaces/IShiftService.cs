using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IShiftService
{
    Task<List<Shift>> GetShiftsForMonthAsync(int year, int month);
    Task<Shift?> GetShiftByDateAsync(DateTime date);
    Task<Shift?> GetShiftByIdAsync(Guid id);
    Task<Shift> GetOrCreateShiftAsync(DateTime date);
    Task UpdateShiftSettingsAsync(Guid shiftId, ShiftType type, int requiredStaffCount, DayStatus dayStatus, string? note);
    Task<(bool Success, string? Error)> SignUpAsync(DateTime date, string userId, string userDisplayName);
    Task<(bool Success, string? Error)> SignOffAsync(DateTime date, string userId);
    Task<(bool Success, string? Error)> OverrideAssignmentAsync(DateTime date, string newUserId, string newUserDisplayName, string existingUserId);
    Task<List<ShiftAssignment>> GetUpcomingAssignmentsForUserAsync(string userId, int count);
    Task<bool> IsUserOnShiftAsync(string userId, DateTime date);
}
