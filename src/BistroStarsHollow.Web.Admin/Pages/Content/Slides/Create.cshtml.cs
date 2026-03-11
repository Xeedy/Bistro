using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Slides;

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
    public HeroSlide Slide { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            ModelState.AddModelError("ImageFile", "Obrázek je povinný.");
            return Page();
        }

        try
        {
            Slide.ImagePath = await _imageService.SaveImageAsync(imageFile, "slides");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("ImageFile", ex.Message);
            return Page();
        }

        Slide.Id = Guid.NewGuid();
        await _contentService.CreateSlideAsync(Slide);
        await _auditService.LogAsync("Create", "HeroSlide", Slide.Id.ToString(), $"Vytvořen slide: {Slide.Title}");

        return RedirectToPage("Index");
    }
}
