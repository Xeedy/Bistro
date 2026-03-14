using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.RateLimiting;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Infrastructure;
using BistroStarsHollow.Infrastructure.Data;
using BistroStarsHollow.Infrastructure.Identity;
using BistroStarsHollow.Web.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ImageUploadService>();
builder.Services.AddInfrastructure(builder.Configuration);

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("global", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

// Antiforgery hardening
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = ".BistroSH.Auth";
    options.Cookie.IsEssential = true;
    if (!builder.Environment.IsDevelopment())
    {
        options.Cookie.Domain = ".bistrosh.cz";
    }
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var seedLogger = services.GetRequiredService<ILogger<Program>>();
        seedLogger.LogError(ex, "An error occurred while seeding the database");
    }
}

// Localization
var supportedCultures = new[] { "cs", "en" }
    .Select(c => new CultureInfo(c))
    .ToList();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("cs"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
localizationOptions.RequestCultureProviders = new IRequestCultureProvider[]
{
    new QueryStringRequestCultureProvider(),
    new CookieRequestCultureProvider(),
    new AcceptLanguageHeaderRequestCultureProvider()
};

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
        "font-src 'self' https://cdn.jsdelivr.net https://fonts.gstatic.com; " +
        "img-src 'self' data:; " +
        "frame-ancestors 'none';";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve shared uploads from solution-level uploads folder
var sharedUploadsPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "uploads"));
if (Directory.Exists(sharedUploadsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(sharedUploadsPath),
        RequestPath = "/uploads"
    });
}

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();

// Development-only: auto-login as admin
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (context.User.Identity?.IsAuthenticated != true
            && !context.Request.Path.StartsWithSegments("/Account"))
        {
            var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            var devEmail = context.RequestServices.GetRequiredService<IConfiguration>()["AdminUser:Email"] ?? "admin@bistrosh.cz";
            var user = await userManager.FindByEmailAsync(devEmail);

            if (user != null)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                context.Response.Redirect(context.Request.Path + context.Request.QueryString);
                return;
            }
        }

        await next();
    });
}

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets()
   .RequireRateLimiting("global");

app.Run();
