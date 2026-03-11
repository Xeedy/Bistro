using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;

namespace BistroStarsHollow.Web.Public.Pages;

public class ReservationCancelModel : PageModel
{
    private readonly IReservationService _reservationService;

    public ReservationCancelModel(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    public bool? CancelSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Token))
        {
            ErrorMessage = "Chybí token pro zrušení rezervace.";
            return Page();
        }

        var (success, error) = await _reservationService.CancelReservationAsync(Token);
        CancelSuccess = success;
        ErrorMessage = error;

        return Page();
    }
}
