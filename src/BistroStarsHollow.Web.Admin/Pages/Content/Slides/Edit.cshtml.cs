using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Slides;

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
    public HeroSlide Slide { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var slide = await _contentService.GetSlideByIdAsync(id);
        if (slide == null) return NotFound();
        Slide = slide;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                _imageService.DeleteImage(Slide.ImagePath);
                Slide.ImagePath = await _imageService.SaveImageAsync(imageFile, "slides");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return Page();
            }
        }

        await _contentService.UpdateSlideAsync(Slide);
        await _auditService.LogAsync("Update", "HeroSlide", Slide.Id.ToString(), $"Upraven slide: {Slide.Title}");

        return RedirectToPage("Index");
    }
}
