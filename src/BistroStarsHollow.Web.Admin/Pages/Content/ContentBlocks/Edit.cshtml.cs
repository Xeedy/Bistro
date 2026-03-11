using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.ContentBlocks;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public EditModel(IContentManagementService contentService, IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public ContentBlock Block { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var block = await _contentService.GetContentBlockByIdAsync(id);
        if (block == null) return NotFound();
        Block = block;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _contentService.UpdateContentBlockAsync(Block);
        await _auditService.LogAsync("Update", "ContentBlock", Block.Id.ToString(), $"Upraven blok: {Block.Key} ({Block.Language})");

        return RedirectToPage("Index");
    }
}
