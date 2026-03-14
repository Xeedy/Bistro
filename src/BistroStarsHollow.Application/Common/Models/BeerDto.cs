using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Models;

public class BeerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BeerType Type { get; set; }
    public string? Style { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceSmall { get; set; }
    public string? Capacity { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public int SortOrder { get; set; }

    public Guid BreweryId { get; set; }
    public string BreweryName { get; set; } = string.Empty;
    public string? BreweryLogoPath { get; set; }

    public string PriceText => $"{Price:0} Kč";
    public string? PriceSmallText => PriceSmall.HasValue ? $"{PriceSmall.Value:0} Kč" : null;
}
