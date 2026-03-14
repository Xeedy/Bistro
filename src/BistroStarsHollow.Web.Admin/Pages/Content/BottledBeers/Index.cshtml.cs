using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Content.BottledBeers;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;
    private readonly IConfiguration _configuration;

    public IndexModel(IContentManagementService contentService, IAuditService auditService, IConfiguration configuration)
    {
        _contentService = contentService;
        _auditService = auditService;
        _configuration = configuration;
    }

    public List<Beer> Beers { get; set; } = new();
    public string PublicUrl { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var allBeers = await _contentService.GetAllBeersAsync();
        Beers = allBeers.Where(b => b.Type == BeerType.Bottled).ToList();
        PublicUrl = _configuration["PublicUrl"] ?? "https://localhost:5000";
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _contentService.ToggleBeerActiveAsync(id);
        var beer = await _contentService.GetBeerByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "Beer", id.ToString(),
            $"Pivo '{beer?.Name}' — {(beer?.IsActive == true ? "aktivováno" : "deaktivováno")}");
        return new JsonResult(new { success = true, isActive = beer?.IsActive });
    }

    public async Task<IActionResult> OnPostUpdateSortOrderAsync([FromBody] List<Guid> orderedIds)
    {
        await _contentService.UpdateBeerSortOrderAsync(orderedIds);
        await _auditService.LogAsync("UpdateSortOrder", "Beer", "", "Změněno pořadí lahvových piv");
        return new JsonResult(new { success = true });
    }
}
