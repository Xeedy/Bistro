using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.BeerStyles;

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

    public List<BeerStyle> BeerStyles { get; set; } = new();

    public async Task OnGetAsync()
    {
        BeerStyles = await _contentService.GetAllBeerStylesAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RedirectToPage();

        var styles = await _contentService.GetAllBeerStylesAsync();
        var maxSort = styles.Count > 0 ? styles.Max(s => s.SortOrder) : 0;
        var style = new BeerStyle
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            SortOrder = maxSort + 1
        };

        await _contentService.CreateBeerStyleAsync(style);
        await _auditService.LogAsync("Create", "BeerStyle", style.Id.ToString(), $"Vytvořen pivní styl: {style.Name}");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRenameAsync(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RedirectToPage();

        var styles = await _contentService.GetAllBeerStylesAsync();
        var style = styles.FirstOrDefault(s => s.Id == id);
        if (style == null) return NotFound();

        style.Name = name.Trim();
        await _contentService.UpdateBeerStyleAsync(style);
        await _auditService.LogAsync("Update", "BeerStyle", id.ToString(), $"Přejmenován pivní styl: {style.Name}");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _contentService.DeleteBeerStyleAsync(id);
        await _auditService.LogAsync("Delete", "BeerStyle", id.ToString(), "Smazán pivní styl");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateSortOrderAsync([FromBody] List<Guid> orderedIds)
    {
        await _contentService.UpdateBeerStyleSortOrderAsync(orderedIds);
        await _auditService.LogAsync("UpdateSortOrder", "BeerStyle", "", "Změněno pořadí pivních stylů");
        return new JsonResult(new { success = true });
    }
}
