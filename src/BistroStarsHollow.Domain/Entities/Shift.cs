using BistroStarsHollow.Domain.Common;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Domain.Entities;

public class Shift : BaseEntity
{
    public DateTime Date { get; set; }
    public ShiftType Type { get; set; } = ShiftType.FullDay;
    public int RequiredStaffCount { get; set; } = 1;
    public DayStatus DayStatus { get; set; } = DayStatus.Open;
    public string? Note { get; set; }

    public ICollection<ShiftAssignment> Assignments { get; set; } = new List<ShiftAssignment>();
}
