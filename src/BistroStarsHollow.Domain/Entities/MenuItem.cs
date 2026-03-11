using BistroStarsHollow.Domain.Common;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Domain.Entities;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public MenuCategory Category { get; set; }
    public decimal? Price { get; set; }
    public string? DisplayPrice { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
