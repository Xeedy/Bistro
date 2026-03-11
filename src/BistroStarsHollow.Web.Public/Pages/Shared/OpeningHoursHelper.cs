using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Public.Pages.Shared;

public static class OpeningHoursHelper
{
    public static string GetDayName(DayOfWeekCz day) => day switch
    {
        DayOfWeekCz.Monday => "Pondělí",
        DayOfWeekCz.Tuesday => "Úterý",
        DayOfWeekCz.Wednesday => "Středa",
        DayOfWeekCz.Thursday => "Čtvrtek",
        DayOfWeekCz.Friday => "Pátek",
        DayOfWeekCz.Saturday => "Sobota",
        DayOfWeekCz.Sunday => "Neděle",
        _ => day.ToString()
    };

    public static string GetShortDayName(DayOfWeekCz day) => day switch
    {
        DayOfWeekCz.Monday => "Po",
        DayOfWeekCz.Tuesday => "Út",
        DayOfWeekCz.Wednesday => "St",
        DayOfWeekCz.Thursday => "Čt",
        DayOfWeekCz.Friday => "Pá",
        DayOfWeekCz.Saturday => "So",
        DayOfWeekCz.Sunday => "Ne",
        _ => day.ToString()
    };
}
