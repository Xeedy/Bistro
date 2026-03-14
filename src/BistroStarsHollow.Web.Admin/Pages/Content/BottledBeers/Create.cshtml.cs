using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.BottledBeers;

[Authorize(Roles = "Admin,Superadmin")]
public class CreateModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly ImageUploadService _imageService;
    private readonly IAuditService _auditService;

    public CreateModel(
        IContentManagementService contentService,
        ImageUploadService imageService,
        IAuditService auditService)
    {
        _contentService = contentService;
        _imageService = imageService;
        _auditService = auditService;
    }

    [BindProperty]
    public Beer Beer { get; set; } = new() { Type = BeerType.Bottled };

    public List<Brewery> Breweries { get; set; } = new();
    public List<BeerStyle> BeerStyles { get; set; } = new();

    public async Task OnGetAsync()
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        BeerStyles = await _contentService.GetAllBeerStylesAsync();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        BeerStyles = await _contentService.GetAllBeerStylesAsync();

        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                Beer.ImagePath = await _imageService.SaveImageAsync(imageFile, "beers");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return Page();
            }
        }

        Beer.Type = BeerType.Bottled;
        Beer.Id = Guid.NewGuid();
        await _contentService.CreateBeerAsync(Beer);
        await _auditService.LogAsync("Create", "Beer", Beer.Id.ToString(), $"Vytvořeno lahvové pivo: {Beer.Name}");

        return RedirectToPage("Index");
    }
}
