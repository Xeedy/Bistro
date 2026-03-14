using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Menu;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;
    private readonly IConfiguration _configuration;

    public IndexModel(IContentManagementService contentService, IAuditService auditService, IConfiguration configuration)
    {
        _contentService = contentService;
        _auditService = auditService;
        _configuration = configuration;
    }

    public List<MenuItem> MenuItems { get; set; } = new();
    public string PublicUrl { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public MenuCategory? FilterCategory { get; set; }

    public async Task OnGetAsync()
    {
        var items = await _contentService.GetAllMenuItemsAsync();

        if (FilterCategory.HasValue)
            items = items.Where(m => m.Category == FilterCategory.Value).ToList();

        MenuItems = items;
        PublicUrl = _configuration["PublicUrl"] ?? "https://localhost:5000";
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _contentService.ToggleMenuItemActiveAsync(id);
        var item = await _contentService.GetMenuItemByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "MenuItem", id.ToString(),
            $"Položka menu '{item?.Name}' — {(item?.IsActive == true ? "aktivována" : "deaktivována")}");
        return new JsonResult(new { success = true, isActive = item?.IsActive });
    }

    public async Task<IActionResult> OnPostUpdateSortOrderAsync([FromBody] List<Guid> orderedIds)
    {
        await _contentService.UpdateMenuItemSortOrderAsync(orderedIds);
        await _auditService.LogAsync("UpdateSortOrder", "MenuItem", "", "Změněno pořadí položek menu");
        return new JsonResult(new { success = true });
    }
}
