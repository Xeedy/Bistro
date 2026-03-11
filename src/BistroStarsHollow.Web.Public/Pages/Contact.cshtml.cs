using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages;

public class ContactModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public ContactModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<OpeningHours> OpeningHoursList { get; set; } = new();

    public async Task OnGetAsync()
    {
        OpeningHoursList = await _publicData.GetOpeningHoursAsync();
    }
}
