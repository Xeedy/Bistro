using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.CashClosures;

[Authorize(Roles = "Admin,Superadmin,Employee")]
public class CreateModel : PageModel
{
    private readonly ICashClosureService _cashClosureService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public CreateModel(ICashClosureService cashClosureService, ICurrentUserService currentUserService, IAuditService auditService)
    {
        _cashClosureService = cashClosureService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
        Input.Date = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var existing = await _cashClosureService.GetByDateAsync(Input.Date);
        if (existing != null)
        {
            ModelState.AddModelError(string.Empty, $"Uzávěrka pro {Input.Date:dd.MM.yyyy} již existuje.");
            return Page();
        }

        var closure = new CashClosure
        {
            Date = Input.Date.Date,
            UserId = _currentUserService.UserId ?? string.Empty,
            CardAmount = Input.CardAmount,
            CashAmount = Input.CashAmount,
            Difference = Input.Difference,
            Note = Input.Note
        };

        await _cashClosureService.CreateAsync(closure);
        await _auditService.LogAsync("Create", "CashClosure", closure.Id.ToString(),
            $"Uzávěrka {Input.Date:dd.MM.yyyy}: celkem {closure.TotalAmount:N2} Kč, odchylka {closure.Difference:N2} Kč");

        TempData["Success"] = $"Uzávěrka pro {Input.Date:dd.MM.yyyy} byla vytvořena.";
        return RedirectToPage("Index");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Datum je povinné.")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Částka kartou je povinná.")]
        [Range(0, double.MaxValue, ErrorMessage = "Částka musí být nezáporná.")]
        public decimal CardAmount { get; set; }

        [Required(ErrorMessage = "Částka hotovost je povinná.")]
        [Range(0, double.MaxValue, ErrorMessage = "Částka musí být nezáporná.")]
        public decimal CashAmount { get; set; }

        public decimal Difference { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
    }
}
