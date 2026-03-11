using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class ContentBlock : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Language { get; set; } = "cs";
}
