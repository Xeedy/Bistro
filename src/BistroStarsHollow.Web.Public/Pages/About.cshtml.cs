using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages;

public class AboutModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public AboutModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public ContentBlock? StoryText { get; set; }
    public ContentBlock? StoryDetail { get; set; }

    public async Task OnGetAsync()
    {
        StoryText = await _publicData.GetContentBlockAsync("about-story");
        StoryDetail = await _publicData.GetContentBlockAsync("about-story-detail");
    }
}
