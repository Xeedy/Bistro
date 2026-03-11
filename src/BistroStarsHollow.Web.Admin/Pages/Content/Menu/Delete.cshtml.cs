using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Menu;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public DeleteModel(
        IContentManagementService contentService,
        IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var item = await _contentService.GetMenuItemByIdAsync(id);
        if (item == null) return NotFound();
        MenuItem = item;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var item = await _contentService.GetMenuItemByIdAsync(MenuItem.Id);
        if (item != null)
        {
            await _contentService.DeleteMenuItemAsync(MenuItem.Id);
            await _auditService.LogAsync("Delete", "MenuItem", MenuItem.Id.ToString(), $"Smazána položka menu: {item.Name}");
        }

        return RedirectToPage("Index");
    }
}
