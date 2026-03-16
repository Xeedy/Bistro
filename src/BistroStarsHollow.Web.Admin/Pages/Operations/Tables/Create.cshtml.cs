using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Tables;

[Authorize(Roles = "Admin,Superadmin")]
public class CreateModel : PageModel
{
    private readonly ITableManagementService _tableService;
    private readonly IAuditService _auditService;

    public CreateModel(ITableManagementService tableService, IAuditService auditService)
    {
        _tableService = tableService;
        _auditService = auditService;
    }

    [BindProperty]
    public BistroTable BistroTable { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        BistroTable.Id = Guid.NewGuid();
        await _tableService.CreateTableAsync(BistroTable);
        await _auditService.LogAsync("Create", "BistroTable", BistroTable.Id.ToString(),
            $"Vytvořen stůl: {BistroTable.Name}");

        return RedirectToPage("Index");
    }
}
