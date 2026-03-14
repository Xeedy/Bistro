using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.CashClosures;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly ICashClosureService _cashClosureService;
    private readonly IAuditService _auditService;

    public DeleteModel(ICashClosureService cashClosureService, IAuditService auditService)
    {
        _cashClosureService = cashClosureService;
        _auditService = auditService;
    }

    public CashClosure Closure { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var closure = await _cashClosureService.GetByIdAsync(id);
        if (closure == null) return NotFound();
        Closure = closure;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var closure = await _cashClosureService.GetByIdAsync(id);
        if (closure != null)
        {
            await _cashClosureService.DeleteAsync(id);
            await _auditService.LogAsync("Delete", "CashClosure", id.ToString(),
                $"Smazána uzávěrka ze dne {closure.Date:dd.MM.yyyy}");
            TempData["Success"] = $"Uzávěrka ze dne {closure.Date:dd.MM.yyyy} byla smazána.";
        }
        return RedirectToPage("Index");
    }
}
