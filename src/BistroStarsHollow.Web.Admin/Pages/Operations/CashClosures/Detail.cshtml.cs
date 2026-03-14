using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.CashClosures;

[Authorize(Roles = "Admin,Superadmin,Employee")]
public class DetailModel : PageModel
{
    private readonly ICashClosureService _cashClosureService;
    private readonly IIdentityService _identityService;

    public DetailModel(ICashClosureService cashClosureService, IIdentityService identityService)
    {
        _cashClosureService = cashClosureService;
        _identityService = identityService;
    }

    public CashClosure Closure { get; set; } = null!;
    public string UserName { get; set; } = "—";

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var closure = await _cashClosureService.GetByIdAsync(id);
        if (closure == null) return RedirectToPage("Index");

        Closure = closure;
        var user = await _identityService.GetUserByIdAsync(closure.UserId);
        if (user != null) UserName = user.FullName;

        return Page();
    }
}
