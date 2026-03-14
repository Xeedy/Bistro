using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Events;

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

    public List<Event> Events { get; set; } = new();

    public async Task OnGetAsync()
    {
        Events = await _contentService.GetAllEventsAsync();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _contentService.ToggleEventActiveAsync(id);
        var evt = await _contentService.GetEventByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "Event", id.ToString(),
            $"Akce '{evt?.Title}' — {(evt?.IsActive == true ? "aktivována" : "deaktivována")}");
        return new JsonResult(new { success = true, isActive = evt?.IsActive });
    }
}
