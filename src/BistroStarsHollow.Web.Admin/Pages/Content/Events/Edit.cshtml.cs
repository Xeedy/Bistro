using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Events;

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
    public Event Event { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var evt = await _contentService.GetEventByIdAsync(id);
        if (evt == null) return NotFound();
        Event = evt;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                _imageService.DeleteImage(Event.ImagePath);
                Event.ImagePath = await _imageService.SaveImageAsync(imageFile, "events");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return Page();
            }
        }

        await _contentService.UpdateEventAsync(Event);
        await _auditService.LogAsync("Update", "Event", Event.Id.ToString(), $"Upravena akce: {Event.Title}");

        return RedirectToPage("Index");
    }
}
