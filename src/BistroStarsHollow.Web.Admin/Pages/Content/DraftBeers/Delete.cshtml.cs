using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.DraftBeers;

[Authorize(Roles = "Admin,Superadmin")]
public class DeleteModel : PageModel
{
    private readonly IContentManagementService _contentService;
    private readonly IAuditService _auditService;

    public DeleteModel(IContentManagementService contentService, IAuditService auditService)
    {
        _contentService = contentService;
        _auditService = auditService;
    }

    [BindProperty]
    public Beer Beer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var beer = await _contentService.GetBeerByIdAsync(id);
        if (beer == null) return NotFound();
        Beer = beer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var beer = await _contentService.GetBeerByIdAsync(Beer.Id);
        if (beer != null)
        {
            await _contentService.DeleteBeerAsync(Beer.Id);
            await _auditService.LogAsync("Delete", "Beer", Beer.Id.ToString(), $"Smazáno čepované pivo: {beer.Name}");
        }
        return RedirectToPage("Index");
    }
}
