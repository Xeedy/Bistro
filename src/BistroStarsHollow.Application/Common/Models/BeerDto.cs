using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Models;

public class BeerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BeerType Type { get; set; }
    public string? Style { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public int SortOrder { get; set; }

    public Guid BreweryId { get; set; }
    public string BreweryName { get; set; } = string.Empty;

    public string PriceText => $"{Price:0} Kč";
}
