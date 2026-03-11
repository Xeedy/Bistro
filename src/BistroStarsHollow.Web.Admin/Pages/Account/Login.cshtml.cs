using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Infrastructure.Identity;

namespace BistroStarsHollow.Web.Admin.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null || !user.IsActive)
        {
            ErrorMessage = "Neplatný e-mail nebo heslo.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(user, Password, RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            ErrorMessage = "Účet je dočasně zablokován. Zkuste to znovu za 5 minut.";
            return Page();
        }

        ErrorMessage = "Neplatný e-mail nebo heslo.";
        return Page();
    }
}
