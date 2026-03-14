using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Gallery;

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

    public List<GalleryImage> Images { get; set; } = new();

    public async Task OnGetAsync()
    {
        Images = await _contentService.GetAllGalleryImagesAsync();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _contentService.ToggleGalleryImageActiveAsync(id);
        var image = await _contentService.GetGalleryImageByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "GalleryImage", id.ToString(),
            $"Obrázek galerie — {(image?.IsActive == true ? "aktivován" : "deaktivován")}");
        return new JsonResult(new { success = true, isActive = image?.IsActive });
    }

    public async Task<IActionResult> OnPostUpdateSortOrderAsync([FromBody] List<Guid> orderedIds)
    {
        await _contentService.UpdateGalleryImageSortOrderAsync(orderedIds);
        await _auditService.LogAsync("UpdateSortOrder", "GalleryImage", "", "Změněno pořadí obrázků galerie");
        return new JsonResult(new { success = true });
    }
}
