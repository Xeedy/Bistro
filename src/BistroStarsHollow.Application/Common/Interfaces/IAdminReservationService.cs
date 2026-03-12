using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IAdminReservationService
{
    Task<List<ReservationListItemDto>> GetReservationsAsync(ReservationFilterDto filter);
    Task<Reservation?> GetReservationByIdAsync(Guid id);
    Task<List<BistroTable>> GetAllTablesAsync();
    Task<int> GetPendingReservationCountAsync();
    Task<(bool Success, string? Error)> ConfirmReservationAsync(Guid id);
    Task<(bool Success, string? Error)> CancelReservationByAdminAsync(Guid id);
    Task<(bool Success, string? Error)> MarkAsCompletedAsync(Guid id);
    Task<(bool Success, string? Error)> MarkAsNoShowAsync(Guid id);
    Task<int> GetTodayReservationCountAsync();
    Task<List<ReservationListItemDto>> GetUpcomingReservationsAsync(int count);
}
