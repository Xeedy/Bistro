using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Gallery;

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
    public GalleryImage Image { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var image = await _contentService.GetGalleryImageByIdAsync(id);
        if (image == null) return NotFound();
        Image = image;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var image = await _contentService.GetGalleryImageByIdAsync(Image.Id);
        if (image != null)
        {
            _imageService.DeleteImage(image.FilePath);
            await _contentService.DeleteGalleryImageAsync(Image.Id);
            await _auditService.LogAsync("Delete", "GalleryImage", Image.Id.ToString(), $"Smazán obrázek: {image.FileName}");
        }

        return RedirectToPage("Index");
    }
}
