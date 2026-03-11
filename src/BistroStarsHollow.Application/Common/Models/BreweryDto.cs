namespace BistroStarsHollow.Application.Common.Models;

public class BreweryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoImagePath { get; set; }
    public int SortOrder { get; set; }

    public int BeerCount { get; set; }
    public List<BeerDto> Beers { get; set; } = new();
}
