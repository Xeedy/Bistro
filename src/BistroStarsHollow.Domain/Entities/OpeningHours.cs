using BistroStarsHollow.Domain.Common;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Domain.Entities;

public class OpeningHours : BaseEntity
{
    public DayOfWeekCz DayOfWeek { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
