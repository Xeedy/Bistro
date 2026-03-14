using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Constants;

namespace BistroStarsHollow.Web.Admin.Pages.Administration.Users;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly IAuditService _auditService;

    public EditModel(IIdentityService identityService, IAuditService auditService)
    {
        _identityService = identityService;
        _auditService = auditService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string[] AvailableRoles => Roles.All;
    public string UserEmail { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _identityService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        UserEmail = user.Email;
        Input = new InputModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            SelectedRoles = user.Roles.ToList()
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var currentUser = await _identityService.GetUserByIdAsync(Input.Id);
            UserEmail = currentUser?.Email ?? "";
            return Page();
        }

        await _identityService.UpdateUserAsync(Input.Id, Input.FirstName, Input.LastName, Input.IsActive);

        // Sync roles
        var currentRoles = await _identityService.GetRolesAsync(Input.Id);
        var desiredRoles = Input.SelectedRoles ?? new List<string>();

        foreach (var role in currentRoles.Except(desiredRoles))
            await _identityService.RemoveFromRoleAsync(Input.Id, role);

        foreach (var role in desiredRoles.Except(currentRoles))
            await _identityService.AddToRoleAsync(Input.Id, role);

        // Optional password reset
        if (!string.IsNullOrWhiteSpace(Input.NewPassword))
            await _identityService.ResetPasswordAsync(Input.Id, Input.NewPassword);

        await _auditService.LogAsync("Update", "User", Input.Id,
            $"Upraven uživatel, role: {string.Join(", ", desiredRoles)}");

        TempData["Success"] = "Uživatel byl upraven.";
        return RedirectToPage("Index");
    }

    public class InputModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Heslo musí mít alespoň 8 znaků.")]
        public string? NewPassword { get; set; }

        public List<string> SelectedRoles { get; set; } = new();
    }
}
