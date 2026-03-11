using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Events;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;

    public IndexModel(IContentManagementService contentService)
    {
        _contentService = contentService;
    }

    public List<Event> Events { get; set; } = new();

    public async Task OnGetAsync()
    {
        Events = await _contentService.GetAllEventsAsync();
    }
}
