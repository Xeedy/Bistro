using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Models;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuCategory Category { get; set; }
    public decimal? Price { get; set; }
    public string? DisplayPrice { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    /// <summary>
    /// Returns the display price string: DisplayPrice if set, otherwise formatted Price, otherwise empty.
    /// </summary>
    public string PriceText =>
        !string.IsNullOrEmpty(DisplayPrice)
            ? DisplayPrice
            : Price.HasValue
                ? $"{Price.Value:0} Kč"
                : string.Empty;
}
