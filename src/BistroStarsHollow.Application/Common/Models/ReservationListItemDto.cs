using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Models;

public class ReservationListItemDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public int NumberOfPeople { get; set; }
    public string TableName { get; set; } = string.Empty;
    public ReservationStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
