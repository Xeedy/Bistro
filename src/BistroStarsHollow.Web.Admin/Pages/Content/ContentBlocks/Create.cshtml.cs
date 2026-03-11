using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.ContentBlocks;

[Authorize(Roles = "Admin,Superadmin")]
public class CreateModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public CreateModel(IContentManagementService contentService, IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public ContentBlock Block { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        Block.Id = Guid.NewGuid();
        await _contentService.CreateContentBlockAsync(Block);
        await _auditService.LogAsync("Create", "ContentBlock", Block.Id.ToString(), $"Vytvořen blok: {Block.Key} ({Block.Language})");

        return RedirectToPage("Index");
    }
}
