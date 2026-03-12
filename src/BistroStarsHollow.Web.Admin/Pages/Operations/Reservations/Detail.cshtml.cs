using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Reservations;

[Authorize(Roles = "Admin,Superadmin,Employee")]
public class DetailModel : PageModel
{
    private readonly IAdminReservationService _reservationService;

    public DetailModel(IAdminReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public Reservation Reservation { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null)
            return RedirectToPage("Index");

        Reservation = reservation;
        return Page();
    }
}
