using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IReservationService
{
    Task<List<TableAvailabilityDto>> GetTableAvailabilityAsync(DateTime date, TimeSpan time, int numberOfPeople);
    Task<(bool Success, ReservationResultDto? Result, string? Error)> CreateReservationAsync(ReservationCreateDto dto, string? ipAddress);
    Task<(bool Success, string? Error)> CancelReservationAsync(string cancelToken);
    Task<bool> IsWithinOpeningHoursAsync(DateTime date, TimeSpan time);
    Task<bool> IsTableAvailableAsync(Guid tableId, DateTime date, TimeSpan time);
    Task<int> GetActiveReservationCountByPhoneAsync(string phone);
}
