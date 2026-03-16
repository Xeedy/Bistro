using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Tables;

[Authorize(Roles = "Admin,Superadmin")]
public class FloorPlanModel : PageModel
{
    private readonly ITableManagementService _tableService;
    private readonly IAuditService _auditService;

    public FloorPlanModel(ITableManagementService tableService, IAuditService auditService)
    {
        _tableService = tableService;
        _auditService = auditService;
    }

    public List<BistroTable> Tables { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tables = await _tableService.GetAllTablesAsync();
    }

    public async Task<IActionResult> OnPostSavePositionsAsync([FromBody] List<TablePositionDto> positions)
    {
        await _tableService.UpdateTablePositionsAsync(positions);
        await _auditService.LogAsync("UpdatePositions", "BistroTable", "",
            $"Aktualizovány pozice {positions.Count} stolů na plánu sálu");
        return new JsonResult(new { success = true });
    }
}
