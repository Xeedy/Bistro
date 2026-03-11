using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class Shift : BaseEntity
{
    public DateTime Date { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Note { get; set; }
}
