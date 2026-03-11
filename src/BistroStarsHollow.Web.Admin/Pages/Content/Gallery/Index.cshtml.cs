using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Web.Admin.Pages.Content.Gallery;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IContentManagementService _contentService;

    public IndexModel(IContentManagementService contentService)
    {
        _contentService = contentService;
    }

    public List<GalleryImage> Images { get; set; } = new();

    public async Task OnGetAsync()
    {
        Images = await _contentService.GetAllGalleryImagesAsync();
    }
}
