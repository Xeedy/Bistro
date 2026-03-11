using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages.Beers;

public class BottledBeersModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public BottledBeersModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<BeerDto> Beers { get; set; } = new();

    public async Task OnGetAsync()
    {
        Beers = await _publicData.GetActiveBeersByTypeAsync(BeerType.Bottled);
    }
}
