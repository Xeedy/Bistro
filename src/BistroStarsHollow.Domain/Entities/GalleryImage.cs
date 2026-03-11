using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class GalleryImage : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public string? AltText { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
