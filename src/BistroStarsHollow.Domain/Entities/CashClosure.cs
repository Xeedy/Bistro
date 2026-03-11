using BistroStarsHollow.Domain.Common;

namespace BistroStarsHollow.Domain.Entities;

public class CashClosure : BaseEntity
{
    public DateTime Date { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal CardAmount { get; set; }
    public decimal CashAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Difference { get; set; }
    public string? Note { get; set; }
}
