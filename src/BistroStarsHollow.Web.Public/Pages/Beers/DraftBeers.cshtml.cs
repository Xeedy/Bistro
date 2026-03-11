using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages.Beers;

public class DraftBeersModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public DraftBeersModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<BeerDto> Beers { get; set; } = new();

    public async Task OnGetAsync()
    {
        Beers = await _publicData.GetActiveBeersByTypeAsync(BeerType.Draft);
    }
}
