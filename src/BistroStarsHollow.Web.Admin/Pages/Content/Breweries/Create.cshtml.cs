using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Breweries;

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
    public Brewery Brewery { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(IFormFile? logoFile)
    {
        if (logoFile != null && logoFile.Length > 0)
        {
            try
            {
                Brewery.LogoImagePath = await _imageService.SaveImageAsync(logoFile, "breweries");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("LogoFile", ex.Message);
                return Page();
            }
        }

        Brewery.Id = Guid.NewGuid();
        await _contentService.CreateBreweryAsync(Brewery);
        await _auditService.LogAsync("Create", "Brewery", Brewery.Id.ToString(), $"Vytvořen pivovar: {Brewery.Name}");

        return RedirectToPage("Index");
    }
}
