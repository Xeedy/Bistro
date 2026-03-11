using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Breweries;

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
    public Brewery Brewery { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var brewery = await _contentService.GetBreweryByIdAsync(id);
        if (brewery == null) return NotFound();
        Brewery = brewery;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? logoFile)
    {
        if (logoFile != null && logoFile.Length > 0)
        {
            try
            {
                _imageService.DeleteImage(Brewery.LogoImagePath);
                Brewery.LogoImagePath = await _imageService.SaveImageAsync(logoFile, "breweries");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("LogoFile", ex.Message);
                return Page();
            }
        }

        await _contentService.UpdateBreweryAsync(Brewery);
        await _auditService.LogAsync("Update", "Brewery", Brewery.Id.ToString(), $"Upraven pivovar: {Brewery.Name}");

        return RedirectToPage("Index");
    }
}
