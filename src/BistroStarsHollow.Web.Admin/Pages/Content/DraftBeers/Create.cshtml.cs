using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Content.DraftBeers;

[Authorize(Roles = "Admin,Superadmin")]
public class CreateModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public CreateModel(IContentManagementService contentService, IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public Beer Beer { get; set; } = new() { Type = BeerType.Draft };

    public List<Brewery> Breweries { get; set; } = new();
    public List<BeerStyle> BeerStyles { get; set; } = new();

    public async Task OnGetAsync()
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        BeerStyles = await _contentService.GetAllBeerStylesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        BeerStyles = await _contentService.GetAllBeerStylesAsync();

        Beer.Type = BeerType.Draft;
        Beer.Id = Guid.NewGuid();
        await _contentService.CreateBeerAsync(Beer);
        await _auditService.LogAsync("Create", "Beer", Beer.Id.ToString(), $"Vytvořeno čepované pivo: {Beer.Name}");

        return RedirectToPage("Index");
    }
}
