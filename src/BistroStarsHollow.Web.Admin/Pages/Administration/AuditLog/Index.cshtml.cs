using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Web.Admin.Pages.Administration.AuditLog;

[Authorize(Roles = "Admin,Superadmin")]
public class IndexModel : PageModel
{
    private readonly IAuditService _auditService;

    public IndexModel(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public IList<AuditLogDto> Logs { get; set; } = new List<AuditLogDto>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? FilterAction { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterFrom { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterTo { get; set; }

    private const int PageSize = 50;

    public async Task OnGetAsync()
    {
        if (CurrentPage < 1) CurrentPage = 1;

        var (logs, totalCount) = await _auditService.GetLogsAsync(
            CurrentPage, PageSize,
            userId: null,
            action: FilterAction,
            fromDate: FilterFrom,
            toDate: FilterTo);

        Logs = logs;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
    }
}
