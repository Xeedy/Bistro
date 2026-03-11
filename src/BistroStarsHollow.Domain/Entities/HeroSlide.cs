using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class HeroSlide : BaseEntity
{
    public string ImagePath { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
