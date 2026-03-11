using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Menu;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public EditModel(
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
        await _contentService.UpdateMenuItemAsync(MenuItem);
        await _auditService.LogAsync("Update", "MenuItem", MenuItem.Id.ToString(), $"Upravena položka menu: {MenuItem.Name}");

        return RedirectToPage("Index");
    }
}
