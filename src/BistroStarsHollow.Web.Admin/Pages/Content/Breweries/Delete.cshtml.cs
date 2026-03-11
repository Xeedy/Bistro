using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Breweries;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly ImageUploadService _imageService;
    private readonly IAuditService _auditService;

    public DeleteModel(
        IContentManagementService contentService,
        ImageUploadService imageService,
        IAuditService auditService)
    {
        _contentService = contentService;
        _imageService = imageService;
        _auditService = auditService;
    }

    [BindProperty]
    public Brewery Brewery { get; set; } = new();

    public int BeerCount { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var brewery = await _contentService.GetBreweryByIdAsync(id);
        if (brewery == null) return NotFound();
        Brewery = brewery;
        BeerCount = await _contentService.GetBreweryBeerCountAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var brewery = await _contentService.GetBreweryByIdAsync(Brewery.Id);
        if (brewery != null)
        {
            _imageService.DeleteImage(brewery.LogoImagePath);
            await _contentService.DeleteBreweryAsync(Brewery.Id);
            await _auditService.LogAsync("Delete", "Brewery", Brewery.Id.ToString(), $"Smazán pivovar: {brewery.Name}");
        }

        return RedirectToPage("Index");
    }
}
