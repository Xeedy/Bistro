using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Slides;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;

    public IndexModel(IContentManagementService contentService)
    {
        _contentService = contentService;
    }

    public List<HeroSlide> Slides { get; set; } = new();

    public async Task OnGetAsync()
    {
        Slides = await _contentService.GetAllSlidesAsync();
    }
}
