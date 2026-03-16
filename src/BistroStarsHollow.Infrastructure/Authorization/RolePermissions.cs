using BistroStarsHollow.Domain.Constants;

namespace BistroStarsHollow.Infrastructure.Authorization;

public static class RolePermissions
{
    public static string[] GetPermissionsForRole(string role)
    {
        return role switch
        {
            Roles.Superadmin => SuperadminPermissions,
            Roles.Admin => AdminPermissions,
            Roles.Employee => EmployeePermissions,
            _ => Array.Empty<string>()
        };
    }

    public static bool RoleHasPermission(string role, string permission)
    {
        var permissions = GetPermissionsForRole(role);
        return permissions.Contains(permission);
    }

    private static readonly string[] SuperadminPermissions = Permissions.All;

    private static readonly string[] AdminPermissions =
    [
        Permissions.Menu.View, Permissions.Menu.Edit, Permissions.Menu.Create, Permissions.Menu.Delete,
        Permissions.Beer.View, Permissions.Beer.Edit, Permissions.Beer.Create, Permissions.Beer.Delete,
        Permissions.Brewery.View, Permissions.Brewery.Edit, Permissions.Brewery.Create, Permissions.Brewery.Delete,
        Permissions.Shift.View, Permissions.Shift.Edit, Permissions.Shift.SignUp,
        Permissions.Gallery.View, Permissions.Gallery.Edit, Permissions.Gallery.Create, Permissions.Gallery.Delete,
        Permissions.Event.View, Permissions.Event.Edit, Permissions.Event.Create, Permissions.Event.Delete,
        Permissions.Content.Edit,
        Permissions.Administration.ManageUsers, Permissions.Administration.ManageRoles,
        Permissions.Administration.ViewAuditLog, Permissions.Administration.ManageOpeningHours
    ];

    private static readonly string[] EmployeePermissions =
    [
        Permissions.Shift.View, Permissions.Shift.SignUp
    ];
}
