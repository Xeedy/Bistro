using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.OpeningHours;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public IndexModel(IContentManagementService contentService, IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public List<Domain.Entities.OpeningHours> Hours { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Hours = await _contentService.GetAllOpeningHoursAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _contentService.UpdateOpeningHoursAsync(Hours);
        await _auditService.LogAsync("Update", "OpeningHours", null, "Aktualizována otevírací doba");

        SuccessMessage = "Otevírací doba byla uložena.";
        return RedirectToPage();
    }
}
