using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Web.Public.Pages;

[EnableRateLimiting("ReservationLimit")]
public class ReservationModel : PageModel
{
    private readonly IReservationService _reservationService;
    private readonly IPublicDataService _publicDataService;

    public ReservationModel(
        IReservationService reservationService,
        IPublicDataService publicDataService)
    {
        _reservationService = reservationService;
        _publicDataService = publicDataService;
    }

    [BindProperty]
    public ReservationCreateDto Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var (success, result, error) = await _reservationService.CreateReservationAsync(Input, ipAddress);

        if (!success)
        {
            ErrorMessage = error;
            return Page();
        }

        TempData["ReservationId"] = result!.ReservationId.ToString();
        TempData["CancelToken"] = result.CancelToken;
        TempData["FirstName"] = result.FirstName;
        TempData["LastName"] = result.LastName;
        TempData["Date"] = result.Date.ToString("yyyy-MM-dd");
        TempData["Time"] = result.Time.ToString(@"hh\:mm");
        TempData["NumberOfPeople"] = result.NumberOfPeople.ToString();
        TempData["TableName"] = result.TableName;

        return RedirectToPage("ReservationConfirmation");
    }

    public async Task<IActionResult> OnGetTablesAsync(DateTime date, TimeSpan time, int numberOfPeople)
    {
        var tables = await _reservationService.GetTableAvailabilityAsync(date, time, numberOfPeople);
        return new JsonResult(tables);
    }

    public async Task<IActionResult> OnGetOpeningHoursAsync()
    {
        var hours = await _publicDataService.GetOpeningHoursAsync();
        var result = hours.Select(h => new
        {
            dayOfWeek = (int)h.DayOfWeek,
            isClosed = h.IsClosed,
            openTime = h.OpenTime?.ToString(@"hh\:mm"),
            closeTime = h.CloseTime?.ToString(@"hh\:mm")
        });
        return new JsonResult(result);
    }
}
