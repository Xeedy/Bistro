using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Beers;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly ImageUploadService _imageService;
    private readonly IAuditService _auditService;

    public EditModel(
        IContentManagementService contentService,
        ImageUploadService imageService,
        IAuditService auditService)
    {
        _contentService = contentService;
        _imageService = imageService;
        _auditService = auditService;
    }

    [BindProperty]
    public Beer Beer { get; set; } = new();

    public List<Brewery> Breweries { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var beer = await _contentService.GetBeerByIdAsync(id);
        if (beer == null) return NotFound();
        Beer = beer;
        Breweries = await _contentService.GetAllBreweriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
    {
        Breweries = await _contentService.GetAllBreweriesAsync();

        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                _imageService.DeleteImage(Beer.ImagePath);
                Beer.ImagePath = await _imageService.SaveImageAsync(imageFile, "beers");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return Page();
            }
        }

        await _contentService.UpdateBeerAsync(Beer);
        await _auditService.LogAsync("Update", "Beer", Beer.Id.ToString(), $"Upraveno pivo: {Beer.Name}");

        return RedirectToPage("Index");
    }
}
