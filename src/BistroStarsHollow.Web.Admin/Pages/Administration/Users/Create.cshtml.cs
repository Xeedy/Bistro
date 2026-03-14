using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Constants;

namespace BistroStarsHollow.Web.Admin.Pages.Administration.Users;

[Authorize(Roles = "Admin,Superadmin")]
public class CreateModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly IAuditService _auditService;

    public CreateModel(IIdentityService identityService, IAuditService auditService)
    {
        _identityService = identityService;
        _auditService = auditService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string[] AvailableRoles => Roles.All;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var roles = Input.SelectedRoles?.ToList() ?? new List<string>();
        if (!roles.Any())
            roles.Add(Roles.Employee);

        var result = await _identityService.CreateUserAsync(
            Input.Email, Input.Password, Input.FirstName, Input.LastName, roles);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        await _auditService.LogAsync("Create", "User", result.User?.Id,
            $"Vytvořen uživatel: {Input.Email} s rolemi: {string.Join(", ", roles)}");

        TempData["Success"] = $"Uživatel {Input.Email} byl vytvořen.";
        return RedirectToPage("Index");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail je povinný.")]
        [EmailAddress(ErrorMessage = "Zadejte platný e-mail.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinné.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Heslo musí mít alespoň 8 znaků.")]
        public string Password { get; set; } = string.Empty;

        public List<string> SelectedRoles { get; set; } = new();
    }
}
