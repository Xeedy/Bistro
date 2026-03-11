using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages.Beers;

public class BreweriesModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public BreweriesModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<BreweryDto> Breweries { get; set; } = new();

    public async Task OnGetAsync()
    {
        Breweries = await _publicData.GetActiveBreweriesAsync();
    }
}
