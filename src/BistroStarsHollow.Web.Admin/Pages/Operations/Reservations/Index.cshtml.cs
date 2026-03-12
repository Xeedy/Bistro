using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Reservations;

[Authorize(Roles = "Admin,Superadmin,Employee")]
public class IndexModel : PageModel
{
    private readonly IAdminReservationService _reservationService;
    private readonly IAuditService _auditService;

    public IndexModel(IAdminReservationService reservationService, IAuditService auditService)
    {
        _reservationService = reservationService;
        _auditService = auditService;
    }

    public List<ReservationListItemDto> Reservations { get; set; } = new();
    public List<SelectListItem> Tables { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? FilterTableId { get; set; }

    [BindProperty(SupportsGet = true)]
    public ReservationStatus? FilterStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchPhrase { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostConfirmAsync(Guid id)
    {
        var (success, error) = await _reservationService.ConfirmReservationAsync(id);
        if (success)
        {
            await _auditService.LogAsync("Reservation.Confirm", "Reservation", id.ToString(), "Rezervace potvrzena");
            TempData["Success"] = "Rezervace byla potvrzena.";
        }
        else
        {
            TempData["Error"] = error;
        }
        return RedirectToPage(new { FilterDate, FilterTableId, FilterStatus, SearchPhrase });
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id)
    {
        var (success, error) = await _reservationService.CancelReservationByAdminAsync(id);
        if (success)
        {
            await _auditService.LogAsync("Reservation.CancelByAdmin", "Reservation", id.ToString(), "Rezervace zrušena adminem");
            TempData["Success"] = "Rezervace byla zrušena.";
        }
        else
        {
            TempData["Error"] = error;
        }
        return RedirectToPage(new { FilterDate, FilterTableId, FilterStatus, SearchPhrase });
    }

    public async Task<IActionResult> OnPostCompleteAsync(Guid id)
    {
        var (success, error) = await _reservationService.MarkAsCompletedAsync(id);
        if (success)
        {
            await _auditService.LogAsync("Reservation.Complete", "Reservation", id.ToString(), "Rezervace dokončena");
            TempData["Success"] = "Rezervace byla označena jako dokončená.";
        }
        else
        {
            TempData["Error"] = error;
        }
        return RedirectToPage(new { FilterDate, FilterTableId, FilterStatus, SearchPhrase });
    }

    public async Task<IActionResult> OnPostNoShowAsync(Guid id)
    {
        var (success, error) = await _reservationService.MarkAsNoShowAsync(id);
        if (success)
        {
            await _auditService.LogAsync("Reservation.NoShow", "Reservation", id.ToString(), "Rezervace označena jako nedostavení");
            TempData["Success"] = "Rezervace byla označena jako nedostavení.";
        }
        else
        {
            TempData["Error"] = error;
        }
        return RedirectToPage(new { FilterDate, FilterTableId, FilterStatus, SearchPhrase });
    }

    private async Task LoadDataAsync()
    {
        var filter = new ReservationFilterDto
        {
            Date = FilterDate,
            TableId = FilterTableId,
            Status = FilterStatus,
            SearchPhrase = SearchPhrase
        };

        Reservations = await _reservationService.GetReservationsAsync(filter);

        var tables = await _reservationService.GetAllTablesAsync();
        Tables = tables.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();

        Statuses = Enum.GetValues<ReservationStatus>()
            .Select(s => new SelectListItem(GetStatusLabel(s), ((int)s).ToString()))
            .ToList();
    }

    public static string GetStatusLabel(ReservationStatus status) => status switch
    {
        ReservationStatus.PendingConfirmation => "Čeká na potvrzení",
        ReservationStatus.Confirmed => "Potvrzena",
        ReservationStatus.CancelledByUser => "Zrušena hostem",
        ReservationStatus.CancelledByAdmin => "Zrušena adminem",
        ReservationStatus.Completed => "Dokončena",
        ReservationStatus.NoShow => "Nedostavení",
        _ => status.ToString()
    };

    public static string GetStatusBadgeClass(ReservationStatus status) => status switch
    {
        ReservationStatus.PendingConfirmation => "bg-warning text-dark",
        ReservationStatus.Confirmed => "bg-success",
        ReservationStatus.CancelledByUser => "bg-secondary",
        ReservationStatus.CancelledByAdmin => "bg-danger",
        ReservationStatus.Completed => "bg-primary",
        ReservationStatus.NoShow => "bg-dark",
        _ => "bg-secondary"
    };
}
