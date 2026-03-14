using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.CashClosures;

[Authorize(Roles = "Admin,Superadmin")]
public class EditModel : PageModel
{
    private readonly ICashClosureService _cashClosureService;
    private readonly IAuditService _auditService;

    public EditModel(ICashClosureService cashClosureService, IAuditService auditService)
    {
        _cashClosureService = cashClosureService;
        _auditService = auditService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var closure = await _cashClosureService.GetByIdAsync(id);
        if (closure == null) return NotFound();

        Input = new InputModel
        {
            Id = closure.Id,
            Date = closure.Date,
            CardAmount = closure.CardAmount,
            CashAmount = closure.CashAmount,
            Difference = closure.Difference,
            Note = closure.Note,
            UserId = closure.UserId
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var closure = await _cashClosureService.GetByIdAsync(Input.Id);
        if (closure == null) return NotFound();

        closure.Date = Input.Date.Date;
        closure.CardAmount = Input.CardAmount;
        closure.CashAmount = Input.CashAmount;
        closure.Difference = Input.Difference;
        closure.Note = Input.Note;

        await _cashClosureService.UpdateAsync(closure);
        await _auditService.LogAsync("Update", "CashClosure", closure.Id.ToString(),
            $"Uzávěrka {Input.Date:dd.MM.yyyy} upravena: celkem {closure.TotalAmount:N2} Kč");

        TempData["Success"] = "Uzávěrka byla upravena.";
        return RedirectToPage("Index");
    }

    public class InputModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CardAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CashAmount { get; set; }

        public decimal Difference { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
    }
}
