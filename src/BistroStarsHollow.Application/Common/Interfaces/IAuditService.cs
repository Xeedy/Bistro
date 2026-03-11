using BistroStarsHollow.Application.Common.Models;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IAuditService
{
    Task<AuditLogDto> LogAsync(
        string action,
        string entityType,
        string? entityId = null,
        string? details = null);

    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
}
