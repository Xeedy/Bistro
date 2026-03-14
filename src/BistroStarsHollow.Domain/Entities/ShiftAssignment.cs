using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class ShiftAssignment : BaseEntity
{
    public Guid ShiftId { get; set; }
    public Shift Shift { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string? Note { get; set; }
}
