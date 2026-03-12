using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Web.Admin.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAdminReservationService _reservationService;

    public IndexModel(ILogger<IndexModel> logger, IAdminReservationService reservationService)
    {
        _logger = logger;
        _reservationService = reservationService;
    }

    public int TodayReservationCount { get; set; }
    public List<ReservationListItemDto> UpcomingReservations { get; set; } = new();

    public async Task OnGetAsync()
    {
        TodayReservationCount = await _reservationService.GetTodayReservationCountAsync();
        UpcomingReservations = await _reservationService.GetUpcomingReservationsAsync(5);
    }
}
