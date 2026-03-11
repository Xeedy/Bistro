namespace BistroStarsHollow.Domain.Constants;

public static class Roles
{
    public const string Superadmin = "Superadmin";
    public const string Admin = "Admin";
    public const string Employee = "Employee";

    public static readonly string[] All =
    [
        Superadmin,
        Admin,
        Employee
    ];
}
