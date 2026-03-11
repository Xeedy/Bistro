namespace BistroStarsHollow.Application.Common.Models;

public class TableAvailabilityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int SortOrder { get; set; }
    public bool IsAvailable { get; set; }
}
