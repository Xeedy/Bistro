namespace BistroStarsHollow.Domain.Enums;

public enum ReservationStatus
{
    PendingConfirmation,
    Confirmed,
    CancelledByUser,
    CancelledByAdmin,
    Completed,
    NoShow
}
