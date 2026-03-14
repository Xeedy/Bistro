using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Web.Admin.Pages.Administration.Users;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public DeleteModel(IIdentityService identityService, IAuditService auditService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }

    public ApplicationUserDto UserToDelete { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _identityService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        UserToDelete = user;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == _currentUserService.UserId)
        {
            TempData["Error"] = "Nemůžete smazat svůj vlastní účet.";
            return RedirectToPage("Index");
        }

        var user = await _identityService.GetUserByIdAsync(id);
        if (user != null)
        {
            await _identityService.DeleteUserAsync(id);
            await _auditService.LogAsync("Delete", "User", id, $"Smazán uživatel: {user.Email}");
            TempData["Success"] = $"Uživatel {user.Email} byl smazán.";
        }

        return RedirectToPage("Index");
    }
}
