using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Gallery;

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
    public GalleryImage Image { get; set; } = new();

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
            var path = await _imageService.SaveImageAsync(imageFile, "gallery");
            Image.FilePath = path;
            Image.FileName = imageFile.FileName;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("ImageFile", ex.Message);
            return Page();
        }

        Image.Id = Guid.NewGuid();
        await _contentService.CreateGalleryImageAsync(Image);
        await _auditService.LogAsync("Create", "GalleryImage", Image.Id.ToString(), $"Přidán obrázek: {Image.FileName}");

        return RedirectToPage("Index");
    }
}
