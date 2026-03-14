using BistroStarsHollow.Domain.Common;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Domain.Entities;

public class Beer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public BeerType Type { get; set; }
    public string? Style { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceSmall { get; set; }
    public string? Capacity { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid BreweryId { get; set; }
    public Brewery Brewery { get; set; } = null!;
}
