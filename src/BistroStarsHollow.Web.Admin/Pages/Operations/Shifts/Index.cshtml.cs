using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Web.Admin.Pages.Operations.Shifts;

[Authorize(Roles = "Admin,Superadmin,Employee")]
public class IndexModel : PageModel
{
    private readonly IShiftService _shiftService;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public IndexModel(
        IShiftService shiftService,
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _shiftService = shiftService;
        _identityService = identityService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    [BindProperty(SupportsGet = true)]
    public int Year { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Month { get; set; }

    public List<Shift> Shifts { get; set; } = new();
    public string CurrentUserId => _currentUserService.UserId ?? string.Empty;
    public string CurrentUserName { get; set; } = string.Empty;
    public string MonthName { get; set; } = string.Empty;
    public List<CalendarDay> CalendarDays { get; set; } = new();

    public async Task OnGetAsync()
    {
        InitializeDate();
        await LoadCalendarAsync();
    }

    public async Task<IActionResult> OnPostSignUpAsync(DateTime date)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var user = await _identityService.GetUserByIdAsync(userId);
        var displayName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Neznámý";

        var (success, error) = await _shiftService.SignUpAsync(date, userId, displayName);
        if (success)
        {
            await _auditService.LogAsync("Shift.SignUp", "Shift", date.ToString("yyyy-MM-dd"),
                $"Zápis na směnu {date:dd.MM.yyyy}");
            TempData["Success"] = $"Byli jste zapsáni na směnu {date:dd.MM.yyyy}.";
        }
        else
        {
            TempData["Error"] = error;
        }

        return RedirectToPage(new { Year = date.Year, Month = date.Month });
    }

    public async Task<IActionResult> OnPostSignOffAsync(DateTime date)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var (success, error) = await _shiftService.SignOffAsync(date, userId);
        if (success)
        {
            await _auditService.LogAsync("Shift.SignOff", "Shift", date.ToString("yyyy-MM-dd"),
                $"Odepsání ze směny {date:dd.MM.yyyy}");
            TempData["Success"] = $"Byli jste odepsáni ze směny {date:dd.MM.yyyy}.";
        }
        else
        {
            TempData["Error"] = error;
        }

        return RedirectToPage(new { Year = date.Year, Month = date.Month });
    }

    public async Task<IActionResult> OnPostOverrideAsync(DateTime date, string existingUserId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var user = await _identityService.GetUserByIdAsync(userId);
        var displayName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Neznámý";

        var (success, error) = await _shiftService.OverrideAssignmentAsync(date, userId, displayName, existingUserId);
        if (success)
        {
            await _auditService.LogAsync("Shift.Override", "Shift", date.ToString("yyyy-MM-dd"),
                $"Přepsání směny {date:dd.MM.yyyy}");
            TempData["Success"] = $"Směna na {date:dd.MM.yyyy} byla přepsána.";
        }
        else
        {
            TempData["Error"] = error;
        }

        return RedirectToPage(new { Year = date.Year, Month = date.Month });
    }

    public async Task<IActionResult> OnPostUpdateSettingsAsync(Guid shiftId, DateTime date, ShiftType type, int requiredStaffCount, DayStatus dayStatus, string? note)
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("Superadmin"))
            return Forbid();

        var shift = await _shiftService.GetOrCreateShiftAsync(date);
        await _shiftService.UpdateShiftSettingsAsync(shift.Id, type, requiredStaffCount, dayStatus, note);

        await _auditService.LogAsync("Shift.UpdateSettings", "Shift", date.ToString("yyyy-MM-dd"),
            $"Aktualizace nastavení směny {date:dd.MM.yyyy}");
        TempData["Success"] = $"Nastavení směny {date:dd.MM.yyyy} bylo aktualizováno.";

        return RedirectToPage(new { Year = date.Year, Month = date.Month });
    }

    private void InitializeDate()
    {
        var now = DateTime.Today;
        if (Year == 0) Year = now.Year;
        if (Month == 0) Month = now.Month;

        var culture = new System.Globalization.CultureInfo("cs-CZ");
        MonthName = culture.DateTimeFormat.GetMonthName(Month);
        MonthName = char.ToUpper(MonthName[0]) + MonthName[1..];
    }

    private async Task LoadCalendarAsync()
    {
        Shifts = await _shiftService.GetShiftsForMonthAsync(Year, Month);

        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await _identityService.GetUserByIdAsync(userId);
            CurrentUserName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "";
        }

        var firstDay = new DateTime(Year, Month, 1);
        var daysInMonth = DateTime.DaysInMonth(Year, Month);

        // Start from Monday
        var startDayOfWeek = (int)firstDay.DayOfWeek;
        if (startDayOfWeek == 0) startDayOfWeek = 7; // Sunday = 7
        var paddingDays = startDayOfWeek - 1; // Monday = 0 padding

        CalendarDays = new List<CalendarDay>();

        // Padding days from previous month
        for (int i = paddingDays; i > 0; i--)
        {
            CalendarDays.Add(new CalendarDay
            {
                Date = firstDay.AddDays(-i),
                IsCurrentMonth = false
            });
        }

        // Days of current month
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(Year, Month, day);
            var shift = Shifts.FirstOrDefault(s => s.Date.Date == date.Date);

            CalendarDays.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth = true,
                IsToday = date == DateTime.Today,
                IsPast = date < DateTime.Today,
                Shift = shift
            });
        }

        // Fill remaining days to complete the week
        var remaining = 7 - (CalendarDays.Count % 7);
        if (remaining < 7)
        {
            var lastDate = new DateTime(Year, Month, daysInMonth);
            for (int i = 1; i <= remaining; i++)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Date = lastDate.AddDays(i),
                    IsCurrentMonth = false
                });
            }
        }
    }

    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsPast { get; set; }
        public Shift? Shift { get; set; }
    }
}
