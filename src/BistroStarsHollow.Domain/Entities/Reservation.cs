using BistroStarsHollow.Domain.Common;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Domain.Entities;

public class Reservation : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public int NumberOfPeople { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.PendingConfirmation;
    public string CancelToken { get; set; } = Guid.NewGuid().ToString("N");
    public string? Note { get; set; }
    public string? IpAddress { get; set; }

    public Guid TableId { get; set; }
    public BistroTable Table { get; set; } = null!;
}
