using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages;

public class GalleryModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public GalleryModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<GalleryImage> Images { get; set; } = new();

    public async Task OnGetAsync()
    {
        Images = await _publicData.GetActiveGalleryImagesAsync();
    }
}
