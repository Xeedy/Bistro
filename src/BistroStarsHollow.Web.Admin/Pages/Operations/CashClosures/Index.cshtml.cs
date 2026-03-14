using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.CashClosures;

[Authorize(Roles = "Admin,Superadmin,Employee")]
public class IndexModel : PageModel
{
    private readonly ICashClosureService _cashClosureService;
    private readonly IIdentityService _identityService;

    public IndexModel(ICashClosureService cashClosureService, IIdentityService identityService)
    {
        _cashClosureService = cashClosureService;
        _identityService = identityService;
    }

    public List<CashClosure> Closures { get; set; } = new();
    public Dictionary<string, string> UserNames { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterFrom { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterTo { get; set; }

    public async Task OnGetAsync()
    {
        Closures = await _cashClosureService.GetByDateRangeAsync(FilterFrom, FilterTo);

        var userIds = Closures.Select(c => c.UserId).Distinct();
        foreach (var userId in userIds)
        {
            var user = await _identityService.GetUserByIdAsync(userId);
            if (user != null)
                UserNames[userId] = user.FullName;
        }
    }
}
