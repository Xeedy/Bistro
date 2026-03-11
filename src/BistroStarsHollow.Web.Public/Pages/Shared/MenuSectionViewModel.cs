using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Web.Public.Pages.Shared;

public class MenuSectionViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-list";
    public string? Subtitle { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}
