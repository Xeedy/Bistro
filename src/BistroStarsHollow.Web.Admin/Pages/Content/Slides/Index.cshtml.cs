using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Slides;

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

    public List<HeroSlide> Slides { get; set; } = new();

    public async Task OnGetAsync()
    {
        Slides = await _contentService.GetAllSlidesAsync();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _contentService.ToggleSlideActiveAsync(id);
        var slide = await _contentService.GetSlideByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "HeroSlide", id.ToString(),
            $"Slide '{slide?.Title}' — {(slide?.IsActive == true ? "aktivován" : "deaktivován")}");
        return new JsonResult(new { success = true, isActive = slide?.IsActive });
    }

    public async Task<IActionResult> OnPostUpdateSortOrderAsync([FromBody] List<Guid> orderedIds)
    {
        await _contentService.UpdateSlideSortOrderAsync(orderedIds);
        await _auditService.LogAsync("UpdateSortOrder", "HeroSlide", "", "Změněno pořadí slidů");
        return new JsonResult(new { success = true });
    }
}
