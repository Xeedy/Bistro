namespace BistroStarsHollow.Application.Common.Models;

public class ReservationResultDto
{
    public Guid ReservationId { get; set; }
    public string CancelToken { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public int NumberOfPeople { get; set; }
    public string TableName { get; set; } = string.Empty;
}
