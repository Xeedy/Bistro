using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Tables;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly ITableManagementService _tableService;
    private readonly IAuditService _auditService;

    public DeleteModel(ITableManagementService tableService, IAuditService auditService)
    {
        _tableService = tableService;
        _auditService = auditService;
    }

    [BindProperty]
    public BistroTable BistroTable { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var table = await _tableService.GetTableByIdAsync(id);
        if (table == null) return NotFound();
        BistroTable = table;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var table = await _tableService.GetTableByIdAsync(BistroTable.Id);
        if (table != null)
        {
            await _tableService.DeleteTableAsync(BistroTable.Id);
            await _auditService.LogAsync("Delete", "BistroTable", BistroTable.Id.ToString(),
                $"Smazán stůl: {table.Name}");
        }

        return RedirectToPage("Index");
    }
}
