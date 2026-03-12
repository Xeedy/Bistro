using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Models;

public class ReservationFilterDto
{
    public DateTime? Date { get; set; }
    public Guid? TableId { get; set; }
    public ReservationStatus? Status { get; set; }
    public string? SearchPhrase { get; set; }
}
