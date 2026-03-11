using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Infrastructure.Data;

namespace BistroStarsHollow.Infrastructure.Services;

public class AuditService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor) : IAuditService
{

    public async Task<AuditLogDto> LogAsync(
        string action,
        string entityType,
        string? entityId = null,
        string? details = null)
    {
        var httpContext = httpContextAccessor.HttpContext;

        var auditLog = new AuditLog
        {
            UserId = currentUserService.UserId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow,
            IpAddress = GetIpAddress(httpContext),
            UserAgent = GetUserAgent(httpContext),
            Details = details
        };

        dbContext.AuditLogs.Add(auditLog);
        await dbContext.SaveChangesAsync();

        return MapToDto(auditLog);
    }

    public async Task<(IList<AuditLogDto> Logs, int TotalCount)> GetLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(al => al.UserId == userId);
        if (!string.IsNullOrEmpty(action))
            query = query.Where(al => al.Action == action);
        if (fromDate.HasValue)
            query = query.Where(al => al.Timestamp >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(al => al.Timestamp <= toDate.Value);

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(al => al.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (logs.Select(MapToDto).ToList(), totalCount);
    }

    private static string? GetIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',').First().Trim();
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static string? GetUserAgent(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
        if (!string.IsNullOrEmpty(userAgent) && userAgent.Length > 500)
            return userAgent[..500];
        return userAgent;
    }

    private static AuditLogDto MapToDto(AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = log.UserName,
            Action = log.Action,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            Timestamp = log.Timestamp,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            Details = log.Details
        };
    }
}
