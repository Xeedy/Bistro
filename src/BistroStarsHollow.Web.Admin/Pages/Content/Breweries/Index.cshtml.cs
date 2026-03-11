using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Breweries;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;

    public IndexModel(IContentManagementService contentService)
    {
        _contentService = contentService;
    }

    public List<Brewery> Breweries { get; set; } = new();
    public Dictionary<Guid, int> BeerCounts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Breweries = await _contentService.GetAllBreweriesAsync();
        foreach (var brewery in Breweries)
        {
            BeerCounts[brewery.Id] = await _contentService.GetBreweryBeerCountAsync(brewery.Id);
        }
    }
}
