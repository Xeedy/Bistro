using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAdminReservationService _reservationService;
    private readonly IShiftService _shiftService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IContentManagementService _contentService;

    public IndexModel(
        ILogger<IndexModel> logger,
        IAdminReservationService reservationService,
        IShiftService shiftService,
        ICurrentUserService currentUserService,
        IContentManagementService contentService)
    {
        _logger = logger;
        _reservationService = reservationService;
        _shiftService = shiftService;
        _currentUserService = currentUserService;
        _contentService = contentService;
    }

    public int TodayReservationCount { get; set; }
    public int ActiveDraftBeerCount { get; set; }
    public int ActiveEventCount { get; set; }
    public List<ReservationListItemDto> UpcomingReservations { get; set; } = new();
    public List<ShiftAssignment> UpcomingShifts { get; set; } = new();
    public bool IsOnShiftToday { get; set; }

    public async Task OnGetAsync()
    {
        TodayReservationCount = await _reservationService.GetTodayReservationCountAsync();
        UpcomingReservations = await _reservationService.GetUpcomingReservationsAsync(5);
        ActiveDraftBeerCount = await _contentService.GetActiveDraftBeerCountAsync();

        var allEvents = await _contentService.GetAllEventsAsync();
        ActiveEventCount = allEvents.Count(e => e.IsActive);

        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            UpcomingShifts = await _shiftService.GetUpcomingAssignmentsForUserAsync(userId, 5);
            IsOnShiftToday = await _shiftService.IsUserOnShiftAsync(userId, DateTime.Today);
        }
    }
}
