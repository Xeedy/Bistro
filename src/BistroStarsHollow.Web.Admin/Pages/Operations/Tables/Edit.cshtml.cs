using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Tables;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly ITableManagementService _tableService;
    private readonly IAuditService _auditService;

    public EditModel(ITableManagementService tableService, IAuditService auditService)
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
        await _tableService.UpdateTableAsync(BistroTable);
        await _auditService.LogAsync("Update", "BistroTable", BistroTable.Id.ToString(),
            $"Upraven stůl: {BistroTable.Name}");

        return RedirectToPage("Index");
    }
}
