using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages;

public class IndexModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public IndexModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<BeerDto> DraftBeers { get; set; } = new();
    public List<StaffOnDuty> Staff { get; set; } = new();
    public List<Event> Events { get; set; } = new();
    public List<GalleryImage> GalleryImages { get; set; } = new();
    public List<BreweryDto> Breweries { get; set; } = new();
    public ContentBlock? AboutText { get; set; }
    public ContentBlock? AboutDetail { get; set; }

    public async Task OnGetAsync()
    {
        DraftBeers = await _publicData.GetActiveBeersByTypeAsync(BeerType.Draft);
        Staff = await _publicData.GetStaffOnDutyAsync(DateTime.UtcNow);
        Events = await _publicData.GetActiveEventsAsync();
        GalleryImages = (await _publicData.GetActiveGalleryImagesAsync()).Take(6).ToList();
        Breweries = await _publicData.GetActiveBreweriesAsync();
        AboutText = await _publicData.GetContentBlockAsync("homepage-about");
        AboutDetail = await _publicData.GetContentBlockAsync("homepage-about-detail");
    }
}
