using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Breweries;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;
    private readonly IConfiguration _configuration;

    public IndexModel(
        IContentManagementService contentService,
        IAuditService auditService,
        IConfiguration configuration)
    {
        _contentService = contentService;
        _auditService = auditService;
        _configuration = configuration;
    }

    public List<Brewery> Breweries { get; set; } = new();
    public Dictionary<Guid, int> BeerCounts { get; set; } = new();
    public string PublicUrl { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        foreach (var brewery in Breweries)
        {
            BeerCounts[brewery.Id] = await _contentService.GetBreweryBeerCountAsync(brewery.Id);
        }
        PublicUrl = _configuration["PublicUrl"] ?? "https://localhost:5000";
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _contentService.ToggleBreweryActiveAsync(id);
        var brewery = await _contentService.GetBreweryByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "Brewery", id.ToString(),
            $"Pivovar '{brewery?.Name}' — {(brewery?.IsActive == true ? "aktivován" : "deaktivován")}");
        return new JsonResult(new { success = true, isActive = brewery?.IsActive });
    }

    public async Task<IActionResult> OnPostUpdateSortOrderAsync([FromBody] List<Guid> orderedIds)
    {
        await _contentService.UpdateBrewerySortOrderAsync(orderedIds);
        await _auditService.LogAsync("UpdateSortOrder", "Brewery", "", "Změněno pořadí pivovarů");
        return new JsonResult(new { success = true });
    }
}
