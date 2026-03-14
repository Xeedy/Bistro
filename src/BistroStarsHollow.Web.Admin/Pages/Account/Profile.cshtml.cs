using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Infrastructure.Identity;

namespace BistroStarsHollow.Web.Admin.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _auditService;

    public ProfileModel(UserManager<ApplicationUser> userManager, IAuditService auditService)
    {
        _userManager = userManager;
        _auditService = auditService;
    }

    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }

    [BindProperty]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public string? CurrentPassword { get; set; }

    [BindProperty]
    public string? NewPassword { get; set; }

    [BindProperty]
    public string? ConfirmPassword { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        LoadUserData(user);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.FirstName = FirstName;
        user.LastName = LastName;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _auditService.LogAsync("Profile.Update", "User", user.Id, "Profil aktualizován");
            TempData["Success"] = "Profil byl úspěšně aktualizován.";
        }
        else
        {
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
        }

        LoadUserData(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword))
        {
            TempData["Error"] = "Vyplňte aktuální i nové heslo.";
            LoadUserData(user);
            return Page();
        }

        if (NewPassword != ConfirmPassword)
        {
            TempData["Error"] = "Nové heslo a potvrzení se neshodují.";
            LoadUserData(user);
            return Page();
        }

        var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
        if (result.Succeeded)
        {
            await _auditService.LogAsync("Profile.ChangePassword", "User", user.Id, "Heslo změněno");
            TempData["Success"] = "Heslo bylo úspěšně změněno.";
        }
        else
        {
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
        }

        LoadUserData(user);
        return Page();
    }

    private async void LoadUserData(ApplicationUser user)
    {
        Email = user.Email ?? "";
        FullName = user.FullName;
        FirstName = user.FirstName;
        LastName = user.LastName;
        CreatedAt = user.CreatedAt;
        Roles = await _userManager.GetRolesAsync(user);
    }
}
