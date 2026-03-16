using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class BistroTable : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int MapX { get; set; }
    public int MapY { get; set; }
    public string? Zone { get; set; }

}
