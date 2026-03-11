using System.Security.Claims;
using BistroStarsHollow.Application.Common.Interfaces;

namespace BistroStarsHollow.Web.Admin.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Roles =>
        User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly()
        ?? new List<string>().AsReadOnly();

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
