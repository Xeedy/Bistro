using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Gallery;

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
    public GalleryImage Image { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var image = await _contentService.GetGalleryImageByIdAsync(id);
        if (image == null) return NotFound();
        Image = image;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                _imageService.DeleteImage(Image.FilePath);
                var path = await _imageService.SaveImageAsync(imageFile, "gallery");
                Image.FilePath = path;
                Image.FileName = imageFile.FileName;
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return Page();
            }
        }

        await _contentService.UpdateGalleryImageAsync(Image);
        await _auditService.LogAsync("Update", "GalleryImage", Image.Id.ToString(), $"Upraven obrázek: {Image.FileName}");

        return RedirectToPage("Index");
    }
}
