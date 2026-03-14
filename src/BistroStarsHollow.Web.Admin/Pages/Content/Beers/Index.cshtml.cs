using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Beers;

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
    public int ActiveDraftBeerCount { get; set; }
    public string PublicUrl { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? Type { get; set; }

    public async Task OnGetAsync()
    {
        var allBeers = await _contentService.GetAllBeersAsync();

        if (Enum.TryParse<BeerType>(Type, true, out var beerType))
        {
            Beers = allBeers.Where(b => b.Type == beerType).ToList();
        }
        else
        {
            Beers = allBeers;
        }

        ActiveDraftBeerCount = await _contentService.GetActiveDraftBeerCountAsync();
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
        await _auditService.LogAsync("UpdateSortOrder", "Beer", "", "Změněno pořadí piv");
        return new JsonResult(new { success = true });
    }
}
