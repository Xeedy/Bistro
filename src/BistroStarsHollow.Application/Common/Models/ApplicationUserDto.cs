namespace BistroStarsHollow.Application.Common.Models;

public class ApplicationUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public IList<string> Roles { get; set; } = new List<string>();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
