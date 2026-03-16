using Microsoft.AspNetCore.Authorization;
using BistroStarsHollow.Domain.Constants;

namespace BistroStarsHollow.Infrastructure.Authorization;

public static class AuthorizationPolicies
{
    public const string RequireSuperadmin = "RequireSuperadmin";
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireEmployee = "RequireEmployee";

    public const string CanManageMenu = "CanManageMenu";
    public const string CanManageBeers = "CanManageBeers";
    public const string CanManageBreweries = "CanManageBreweries";
    public const string CanManageShifts = "CanManageShifts";
    public const string CanSignUpShifts = "CanSignUpShifts";
    public const string CanManageGallery = "CanManageGallery";
    public const string CanManageEvents = "CanManageEvents";
    public const string CanEditContent = "CanEditContent";
    public const string CanManageUsers = "CanManageUsers";
    public const string CanViewAuditLog = "CanViewAuditLog";
    public const string CanManageOpeningHours = "CanManageOpeningHours";
    public const string CanViewSystemLog = "CanViewSystemLog";

    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(RequireSuperadmin, policy =>
            policy.RequireRole(Roles.Superadmin));

        options.AddPolicy(RequireAdmin, policy =>
            policy.RequireRole(Roles.Superadmin, Roles.Admin));

        options.AddPolicy(RequireEmployee, policy =>
            policy.RequireRole(Roles.Superadmin, Roles.Admin, Roles.Employee));

        options.AddPolicy(CanManageMenu, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Menu.Edit)));

        options.AddPolicy(CanManageBeers, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Beer.Edit)));

        options.AddPolicy(CanManageBreweries, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Brewery.Edit)));

        options.AddPolicy(CanManageShifts, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Shift.Edit)));

        options.AddPolicy(CanSignUpShifts, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Shift.SignUp)));

        options.AddPolicy(CanManageGallery, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Gallery.Edit)));

        options.AddPolicy(CanManageEvents, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Event.Edit)));

        options.AddPolicy(CanEditContent, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Content.Edit)));

        options.AddPolicy(CanManageUsers, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Administration.ManageUsers)));

        options.AddPolicy(CanViewAuditLog, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Administration.ViewAuditLog)));

        options.AddPolicy(CanManageOpeningHours, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Administration.ManageOpeningHours)));

        options.AddPolicy(CanViewSystemLog, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.System.ViewSystemLog)));
    }
}
