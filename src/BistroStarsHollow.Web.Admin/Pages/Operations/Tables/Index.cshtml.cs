using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Tables;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly ITableManagementService _tableService;
    private readonly IAuditService _auditService;

    public IndexModel(ITableManagementService tableService, IAuditService auditService)
    {
        _tableService = tableService;
        _auditService = auditService;
    }

    public List<BistroTable> Tables { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tables = await _tableService.GetAllTablesAsync();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        await _tableService.ToggleTableActiveAsync(id);
        var table = await _tableService.GetTableByIdAsync(id);
        await _auditService.LogAsync("ToggleActive", "BistroTable", id.ToString(),
            $"Stůl '{table?.Name}' — {(table?.IsActive == true ? "aktivován" : "deaktivován")}");
        return new JsonResult(new { success = true, isActive = table?.IsActive });
    }
}
