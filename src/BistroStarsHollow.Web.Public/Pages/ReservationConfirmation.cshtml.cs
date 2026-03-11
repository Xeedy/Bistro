using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages;

public class ReservationConfirmationModel : PageModel
{
    public string? ReservationId { get; set; }
    public string? CancelToken { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? NumberOfPeople { get; set; }
    public string? TableName { get; set; }
    public string? CancelUrl { get; set; }

    public IActionResult OnGet()
    {
        ReservationId = TempData["ReservationId"]?.ToString();
        CancelToken = TempData["CancelToken"]?.ToString();
        FirstName = TempData["FirstName"]?.ToString();
        LastName = TempData["LastName"]?.ToString();
        Date = TempData["Date"]?.ToString();
        Time = TempData["Time"]?.ToString();
        NumberOfPeople = TempData["NumberOfPeople"]?.ToString();
        TableName = TempData["TableName"]?.ToString();

        if (string.IsNullOrEmpty(ReservationId))
            return RedirectToPage("Reservation");

        CancelUrl = Url.Page("ReservationCancel", new { token = CancelToken });

        return Page();
    }
}
