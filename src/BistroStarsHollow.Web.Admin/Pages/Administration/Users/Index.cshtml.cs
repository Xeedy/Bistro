using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Web.Admin.Pages.Administration.Users;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly IAuditService _auditService;

    public IndexModel(IIdentityService identityService, IAuditService auditService)
    {
        _identityService = identityService;
        _auditService = auditService;
    }

    public IList<ApplicationUserDto> Users { get; set; } = new List<ApplicationUserDto>();

    [BindProperty(SupportsGet = true)]
    public string? SearchPhrase { get; set; }

    public async Task OnGetAsync()
    {
        var users = await _identityService.GetAllUsersAsync();

        if (!string.IsNullOrWhiteSpace(SearchPhrase))
        {
            var search = SearchPhrase.Trim().ToLower();
            users = users.Where(u =>
                (u.FullName?.ToLower().Contains(search) == true) ||
                (u.Email?.ToLower().Contains(search) == true)).ToList();
        }

        Users = users;
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(string id)
    {
        var user = await _identityService.GetUserByIdAsync(id);
        if (user != null)
        {
            await _identityService.UpdateUserAsync(id, user.FirstName, user.LastName, !user.IsActive);
            await _auditService.LogAsync("ToggleActive", "User", id,
                $"Uživatel '{user.Email}' — {(!user.IsActive ? "aktivován" : "deaktivován")}");
        }
        return RedirectToPage(new { SearchPhrase });
    }
}
