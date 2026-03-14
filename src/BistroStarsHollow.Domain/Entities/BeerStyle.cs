using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class BeerStyle : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
