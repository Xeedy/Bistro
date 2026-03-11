using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class StaffOnDuty : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime Date { get; set; }
}
