using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BistroStarsHollow.Web.Public.ViewComponents;

public class OpeningHoursViewComponent : ViewComponent
{
    private readonly IPublicDataService _publicData;

    public OpeningHoursViewComponent(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var hours = await _publicData.GetOpeningHoursAsync();
        return View(hours);
    }
}
