using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Web.Admin.Services;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Events;

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
    public Event Event { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var evt = await _contentService.GetEventByIdAsync(id);
        if (evt == null) return NotFound();
        Event = evt;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var evt = await _contentService.GetEventByIdAsync(Event.Id);
        if (evt != null)
        {
            _imageService.DeleteImage(evt.ImagePath);
            await _contentService.DeleteEventAsync(Event.Id);
            await _auditService.LogAsync("Delete", "Event", Event.Id.ToString(), $"Smazána akce: {evt.Title}");
        }

        return RedirectToPage("Index");
    }
}
