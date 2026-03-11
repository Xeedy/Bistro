using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Slides;

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
    public HeroSlide Slide { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var slide = await _contentService.GetSlideByIdAsync(id);
        if (slide == null) return NotFound();
        Slide = slide;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var slide = await _contentService.GetSlideByIdAsync(Slide.Id);
        if (slide != null)
        {
            _imageService.DeleteImage(slide.ImagePath);
            await _contentService.DeleteSlideAsync(Slide.Id);
            await _auditService.LogAsync("Delete", "HeroSlide", Slide.Id.ToString(), $"Smazán slide: {slide.Title}");
        }

        return RedirectToPage("Index");
    }
}
