using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Content.DraftBeers;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public EditModel(IContentManagementService contentService, IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public Beer Beer { get; set; } = new();

    public List<Brewery> Breweries { get; set; } = new();
    public List<BeerStyle> BeerStyles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var beer = await _contentService.GetBeerByIdAsync(id);
        if (beer == null) return NotFound();
        Beer = beer;
        Breweries = await _contentService.GetAllBreweriesAsync();
        BeerStyles = await _contentService.GetAllBeerStylesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        BeerStyles = await _contentService.GetAllBeerStylesAsync();

        Beer.Type = BeerType.Draft;
        await _contentService.UpdateBeerAsync(Beer);
        await _auditService.LogAsync("Update", "Beer", Beer.Id.ToString(), $"Upraveno čepované pivo: {Beer.Name}");

        return RedirectToPage("Index");
    }
}
