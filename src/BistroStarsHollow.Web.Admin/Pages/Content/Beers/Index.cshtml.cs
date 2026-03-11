using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Beers;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;

    public IndexModel(IContentManagementService contentService)
    {
        _contentService = contentService;
    }

    public List<Beer> Beers { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Type { get; set; }

    public async Task OnGetAsync()
    {
        var allBeers = await _contentService.GetAllBeersAsync();

        if (Enum.TryParse<BeerType>(Type, true, out var beerType))
        {
            Beers = allBeers.Where(b => b.Type == beerType).ToList();
        }
        else
        {
            Beers = allBeers;
        }
    }
}
