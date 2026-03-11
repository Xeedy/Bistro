using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages.Beers;

public class BreweryDetailModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public BreweryDetailModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public BreweryDto? Brewery { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
        {
            return RedirectToPage("/Beers/Breweries");
        }

        Brewery = await _publicData.GetBreweryByIdAsync(id.Value);

        if (Brewery == null)
        {
            return RedirectToPage("/Beers/Breweries");
        }

        return Page();
    }
}
