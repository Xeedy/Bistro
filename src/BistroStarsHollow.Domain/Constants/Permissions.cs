namespace BistroStarsHollow.Domain.Constants;

public static class Permissions
{
    public static class Menu
    {
        public const string View = "Permissions.Menu.View";
        public const string Edit = "Permissions.Menu.Edit";
        public const string Create = "Permissions.Menu.Create";
        public const string Delete = "Permissions.Menu.Delete";
    }

    public static class Beer
    {
        public const string View = "Permissions.Beer.View";
        public const string Edit = "Permissions.Beer.Edit";
        public const string Create = "Permissions.Beer.Create";
        public const string Delete = "Permissions.Beer.Delete";
    }

    public static class Brewery
    {
        public const string View = "Permissions.Brewery.View";
        public const string Edit = "Permissions.Brewery.Edit";
        public const string Create = "Permissions.Brewery.Create";
        public const string Delete = "Permissions.Brewery.Delete";
    }

    public static class Reservation
    {
        public const string View = "Permissions.Reservation.View";
        public const string Edit = "Permissions.Reservation.Edit";
        public const string Create = "Permissions.Reservation.Create";
        public const string Delete = "Permissions.Reservation.Delete";
    }

    public static class Shift
    {
        public const string View = "Permissions.Shift.View";
        public const string Edit = "Permissions.Shift.Edit";
        public const string SignUp = "Permissions.Shift.SignUp";
    }

    public static class Gallery
    {
        public const string View = "Permissions.Gallery.View";
        public const string Edit = "Permissions.Gallery.Edit";
        public const string Create = "Permissions.Gallery.Create";
        public const string Delete = "Permissions.Gallery.Delete";
    }

    public static class Event
    {
        public const string View = "Permissions.Event.View";
        public const string Edit = "Permissions.Event.Edit";
        public const string Create = "Permissions.Event.Create";
        public const string Delete = "Permissions.Event.Delete";
    }

    public static class Content
    {
        public const string Edit = "Permissions.Content.Edit";
    }

    public static class Administration
    {
        public const string ManageUsers = "Permissions.Administration.ManageUsers";
        public const string ManageRoles = "Permissions.Administration.ManageRoles";
        public const string ViewAuditLog = "Permissions.Administration.ViewAuditLog";
        public const string ManageOpeningHours = "Permissions.Administration.ManageOpeningHours";
    }

    public static class System
    {
        public const string SystemAdmin = "Permissions.System.SystemAdmin";
        public const string ViewSystemLog = "Permissions.System.ViewSystemLog";
        public const string ViewAnalytics = "Permissions.System.ViewAnalytics";
    }

    public static readonly string[] All =
    [
        Menu.View, Menu.Edit, Menu.Create, Menu.Delete,
        Beer.View, Beer.Edit, Beer.Create, Beer.Delete,
        Brewery.View, Brewery.Edit, Brewery.Create, Brewery.Delete,
        Reservation.View, Reservation.Edit, Reservation.Create, Reservation.Delete,
        Shift.View, Shift.Edit, Shift.SignUp,
        Gallery.View, Gallery.Edit, Gallery.Create, Gallery.Delete,
        Event.View, Event.Edit, Event.Create, Event.Delete,
        Content.Edit,
        Administration.ManageUsers, Administration.ManageRoles,
        Administration.ViewAuditLog, Administration.ManageOpeningHours,
        System.SystemAdmin, System.ViewSystemLog, System.ViewAnalytics
    ];
}
