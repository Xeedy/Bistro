namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
