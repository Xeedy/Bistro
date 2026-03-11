using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Menu;

[Authorize(Roles = "Admin,Superadmin")]
public class CreateModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public CreateModel(
        IContentManagementService contentService,
        IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        MenuItem.Id = Guid.NewGuid();
        await _contentService.CreateMenuItemAsync(MenuItem);
        await _auditService.LogAsync("Create", "MenuItem", MenuItem.Id.ToString(), $"Vytvořena položka menu: {MenuItem.Name}");

        return RedirectToPage("Index");
    }
}
