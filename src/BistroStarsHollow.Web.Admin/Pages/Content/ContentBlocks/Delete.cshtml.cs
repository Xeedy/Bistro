using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.ContentBlocks;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public DeleteModel(IContentManagementService contentService, IAuditService auditService)
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
        var block = await _contentService.GetContentBlockByIdAsync(Block.Id);
        if (block != null)
        {
            await _contentService.DeleteContentBlockAsync(Block.Id);
            await _auditService.LogAsync("Delete", "ContentBlock", Block.Id.ToString(), $"Smazán blok: {block.Key} ({block.Language})");
        }

        return RedirectToPage("Index");
    }
}
