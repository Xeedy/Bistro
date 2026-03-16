using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BistroStarsHollow.Domain.Constants;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using BistroStarsHollow.Infrastructure.Identity;

namespace BistroStarsHollow.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await SeedRolesAsync(roleManager, logger);
        await SeedAdminUserAsync(userManager, configuration, logger);

        await SeedBeerStylesAsync(dbContext, logger);

        if (environment.IsDevelopment())
        {
            await SeedTestDataAsync(dbContext, userManager, logger);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    logger.LogInformation("Created role: {RoleName}", roleName);
                else
                    logger.LogError("Failed to create role {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@bistrosh.cz";
        var adminPassword = configuration["AdminUser:Password"] ?? "Admin123!";
        var adminFirstName = configuration["AdminUser:FirstName"] ?? "Systém";
        var adminLastName = configuration["AdminUser:LastName"] ?? "Administrátor";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            logger.LogInformation("Admin user already exists: {Email}", adminEmail);
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = adminFirstName,
            LastName = adminLastName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Superadmin);
            logger.LogInformation("Created admin user: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedBeerStylesAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        if (!await dbContext.BeerStyles.AnyAsync())
        {
            var styles = new List<BeerStyle>
            {
                new() { Name = "IPA", SortOrder = 1 },
                new() { Name = "NEIPA", SortOrder = 2 },
                new() { Name = "APA", SortOrder = 3 },
                new() { Name = "Lager", SortOrder = 4 },
                new() { Name = "Pilsner", SortOrder = 5 },
                new() { Name = "Stout", SortOrder = 6 },
                new() { Name = "Porter", SortOrder = 7 },
                new() { Name = "Wheat", SortOrder = 8 },
                new() { Name = "Sour", SortOrder = 9 },
                new() { Name = "Pale Ale", SortOrder = 10 },
                new() { Name = "Světlý ležák", SortOrder = 11 },
                new() { Name = "Tmavý ležák", SortOrder = 12 },
            };
            dbContext.BeerStyles.AddRange(styles);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded beer styles");
        }
    }

    private static async Task SeedTestDataAsync(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        // Seed test employee
        var employeeEmail = "zamestnanec@bistrosh.cz";
        if (await userManager.FindByEmailAsync(employeeEmail) == null)
        {
            var employee = new ApplicationUser
            {
                UserName = employeeEmail,
                Email = employeeEmail,
                FirstName = "Jan",
                LastName = "Novák",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(employee, "Zamestnanec123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employee, Roles.Employee);
                logger.LogInformation("Created test employee: {Email}", employeeEmail);
            }
        }

        // Seed test admin
        var adminEmail = "spravce@bistrosh.cz";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Marie",
                LastName = "Správcová",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(admin, "Spravce123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                logger.LogInformation("Created test admin: {Email}", adminEmail);
            }
        }

        // Seed opening hours
        if (!await dbContext.OpeningHours.AnyAsync())
        {
            var openingHours = new List<OpeningHours>
            {
                new() { DayOfWeek = DayOfWeekCz.Monday, IsClosed = true },
                new() { DayOfWeek = DayOfWeekCz.Tuesday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
                new() { DayOfWeek = DayOfWeekCz.Wednesday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
                new() { DayOfWeek = DayOfWeekCz.Thursday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
                new() { DayOfWeek = DayOfWeekCz.Friday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(23, 0, 0) },
                new() { DayOfWeek = DayOfWeekCz.Saturday, OpenTime = new TimeSpan(12, 0, 0), CloseTime = new TimeSpan(23, 0, 0) },
                new() { DayOfWeek = DayOfWeekCz.Sunday, OpenTime = new TimeSpan(12, 0, 0), CloseTime = new TimeSpan(21, 0, 0) }
            };
            dbContext.OpeningHours.AddRange(openingHours);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded opening hours");
        }

        // Seed tables
        if (!await dbContext.BistroTables.AnyAsync())
        {
            var tables = new List<BistroTable>
            {
                new() { Name = "Stůl 1", Capacity = 4, SortOrder = 1, MapX = 15, MapY = 2 },
                new() { Name = "Stůl 2", Capacity = 4, SortOrder = 2, MapX = 21, MapY = 2 },
                new() { Name = "Stůl 3", Capacity = 4, SortOrder = 3, MapX = 27, MapY = 2 },
                new() { Name = "Stůl 4", Capacity = 2, SortOrder = 4, MapX = 15, MapY = 7 },
                new() { Name = "Stůl 5", Capacity = 6, SortOrder = 5, MapX = 27, MapY = 8 },
                new() { Name = "Stůl 6", Capacity = 6, SortOrder = 6, MapX = 15, MapY = 13 },
                new() { Name = "Stůl 7", Capacity = 4, SortOrder = 7, MapX = 27, MapY = 14 },
                new() { Name = "Stůl 8", Capacity = 6, SortOrder = 8, MapX = 22, MapY = 21 },
                new() { Name = "Stůl 9", Capacity = 2, SortOrder = 9, MapX = 2, MapY = 21 }
            };
            dbContext.BistroTables.AddRange(tables);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded bistro tables");
        }

        // Seed breweries and beers
        if (!await dbContext.Breweries.AnyAsync())
        {
            var radegast = new Brewery { Name = "Pivovar Radegast", Description = "Tradiční moravský pivovar z Nošovic pod Beskydami. Vaří pivo od roku 1970.", SortOrder = 1, IsActive = true };
            var matuska = new Brewery { Name = "Pivovar Matuška", Description = "Řemeslný pivovar z Broumy, jeden z průkopníků české craft beer scény.", SortOrder = 2, IsActive = true };
            var raven = new Brewery { Name = "Raven Brewery", Description = "Moderní český minipivovar zaměřený na americké pivní styly. Sídlí v Plzni.", SortOrder = 3, IsActive = true };
            var zichovec = new Brewery { Name = "Pivovar Zichovec", Description = "Rodinný pivovar z Loun, specialista na moderní pivní styly jako NEIPA a sour ale.", SortOrder = 4, IsActive = true };
            var falkon = new Brewery { Name = "Pivovar Falkon", Description = "Minipivovar ze Žatce, města chmele. Vaří tradiční české styly.", SortOrder = 5, IsActive = true };
            var clock = new Brewery { Name = "Clock Brewery", Description = "Pivovar z Potštejna, kombinující české tradice s moderními trendy.", SortOrder = 6, IsActive = true };

            dbContext.Breweries.AddRange(radegast, matuska, raven, zichovec, falkon, clock);
            await dbContext.SaveChangesAsync();

            var beers = new List<Beer>
            {
                // Draft beers
                new() { Name = "Radegast 12°", Type = BeerType.Draft, Style = "Světlý ležák", Price = 45, PriceSmall = 32, BreweryId = radegast.Id, SortOrder = 1, IsActive = true },
                new() { Name = "Radegast Rázná 10", Type = BeerType.Draft, Style = "Světlé výčepní", Price = 35, PriceSmall = 25, BreweryId = radegast.Id, SortOrder = 2, IsActive = true },
                new() { Name = "Birell", Type = BeerType.Draft, Style = "Nealkoholické", Price = 35, PriceSmall = 25, BreweryId = radegast.Id, SortOrder = 3, IsActive = true },
                new() { Name = "Velkopopovický Kozel 11°", Type = BeerType.Draft, Style = "Světlý ležák", Price = 42, PriceSmall = 30, BreweryId = radegast.Id, SortOrder = 4, IsActive = true, Description = "Klasický český ležák s plnou chutí." },

                // Bottled beers
                new() { Name = "Matuška Raptor IPA", Type = BeerType.Bottled, Style = "IPA", Price = 79, BreweryId = matuska.Id, SortOrder = 1, IsActive = true, Description = "Intenzivní chmelový charakter s tóny tropického ovoce." },
                new() { Name = "Raven American Pale Ale", Type = BeerType.Bottled, Style = "APA", Price = 69, BreweryId = raven.Id, SortOrder = 2, IsActive = true, Description = "Lehce hořký s citrusovými tóny." },
                new() { Name = "Zichovec Nectar of Happiness", Type = BeerType.Bottled, Style = "NEIPA", Price = 89, BreweryId = zichovec.Id, SortOrder = 3, IsActive = true, Description = "Šťavnatá a ovocná New England IPA." },
                new() { Name = "Falkon Tmavý ležák 13°", Type = BeerType.Bottled, Style = "Tmavý ležák", Price = 65, BreweryId = falkon.Id, SortOrder = 4, IsActive = true, Description = "Plný a karamelový tmavý ležák." },
                new() { Name = "Clock Bohemian Pilsner", Type = BeerType.Bottled, Style = "Český pilsner", Price = 59, BreweryId = clock.Id, SortOrder = 5, IsActive = true, Description = "Klasický český pilsner s jemnou hořkostí." },
                new() { Name = "Sibeeria Mango Lassi Sour", Type = BeerType.Bottled, Style = "Sour Ale", Price = 95, BreweryId = zichovec.Id, SortOrder = 6, IsActive = true, Description = "Kyselé pivo s mangem a jogurtovým charakterem." },
            };
            dbContext.Beers.AddRange(beers);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded breweries and beers");
        }

        // Seed menu items
        if (!await dbContext.MenuItems.AnyAsync())
        {
            var menuItems = new List<MenuItem>
            {
                // Food
                new() { Name = "Hovězí burger s hranolkami", Description = "200g hovězí, cheddar, slanina, salát", Price = 189, Category = MenuCategory.Food, SortOrder = 1 },
                new() { Name = "Kuřecí stripsy s dipem", Description = "Křupavé kuřecí kousky, BBQ dip", Price = 139, Category = MenuCategory.Food, SortOrder = 2 },
                new() { Name = "Nachos s guacamole", Description = "Tortilla chipsy, guacamole, salsa, sýr", Price = 119, Category = MenuCategory.Food, SortOrder = 3 },
                new() { Name = "Palačinka slaná – šunka & sýr", Description = "Domácí palačinka s pražskou šunkou a eidamem", Price = 99, Category = MenuCategory.Food, SortOrder = 4 },
                new() { Name = "Palačinka sladká – Nutella & banán", Description = "Domácí palačinka s Nutellou a čerstvým banánem", Price = 89, Category = MenuCategory.Food, SortOrder = 5 },
                new() { Name = "Palačinka sladká – jahody & šlehačka", Description = "Domácí palačinka, čerstvé jahody, šlehačka", Price = 99, Category = MenuCategory.Food, SortOrder = 6 },

                // Cold Drinks
                new() { Name = "Coca-Cola / Fanta / Sprite", Description = "0,33l", Price = 39, Category = MenuCategory.ColdDrinks, SortOrder = 1 },
                new() { Name = "Domácí limonáda", Description = "Citrón & máta / Zázvor & med", Price = 59, Category = MenuCategory.ColdDrinks, SortOrder = 2 },
                new() { Name = "Džus pomeranč / jablko", Description = "0,25l", Price = 45, Category = MenuCategory.ColdDrinks, SortOrder = 3 },
                new() { Name = "Espresso", Price = 49, Category = MenuCategory.ColdDrinks, SortOrder = 4 },
                new() { Name = "Cappuccino", Price = 59, Category = MenuCategory.ColdDrinks, SortOrder = 5 },
                new() { Name = "Latte Macchiato", Price = 65, Category = MenuCategory.ColdDrinks, SortOrder = 6 },
                new() { Name = "Minerální voda", Description = "Perlivá / Neperlivá, 0,33l", Price = 29, Category = MenuCategory.ColdDrinks, SortOrder = 7 },

                // Water pipes
                new() { Name = "Klasická vodní dýmka", Description = "Výběr z nabídky příchutí", Price = 250, Category = MenuCategory.WaterPipes, SortOrder = 1 },
                new() { Name = "Premium vodní dýmka", Description = "Prémiové tabákové směsi", Price = 350, Category = MenuCategory.WaterPipes, SortOrder = 2 },
                new() { Name = "Ovocná miska", Description = "Vodní dýmka servírovaná v ovoci", Price = 400, Category = MenuCategory.WaterPipes, SortOrder = 3 },
                new() { Name = "Extra uhlíky", Price = 30, Category = MenuCategory.WaterPipes, SortOrder = 4 },

                // Tea
                new() { Name = "Zelený čaj – Gunpowder", Description = "Sypaný, konvička", Price = 55, Category = MenuCategory.Tea, SortOrder = 1 },
                new() { Name = "Černý čaj – Earl Grey", Description = "Sypaný, konvička", Price = 55, Category = MenuCategory.Tea, SortOrder = 2 },
                new() { Name = "Ovocný čaj", Description = "Sypaný, konvička", Price = 49, Category = MenuCategory.Tea, SortOrder = 3 },
                new() { Name = "Bylinkový čaj – Máta", Description = "Čerstvá máta, konvička", Price = 55, Category = MenuCategory.Tea, SortOrder = 4 },
                new() { Name = "Zázvorový čaj s medem", Description = "Čerstvý zázvor, med, citrón", Price = 59, Category = MenuCategory.Tea, SortOrder = 5 },

                // Products (sale items)
                new() { Name = "Lahvové pivo – výběr dle nabídky", Description = "Craft piva z regionálních pivovarů", DisplayPrice = "od 59 Kč", Category = MenuCategory.Products, SortOrder = 1 },
                new() { Name = "Domácí sirup", Description = "Bezový / Mátový / Zázvorový, 0,5l", Price = 89, Category = MenuCategory.Products, SortOrder = 2 },
            };
            dbContext.MenuItems.AddRange(menuItems);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded menu items");
        }

        // Seed events
        if (!await dbContext.Events.AnyAsync())
        {
            var events = new List<Event>
            {
                new() { Title = "Živá hudba – Akustický večer", Description = "Přijďte si poslechnout lokální kapelu v uvolněné atmosféře. Vstup zdarma, rezervace doporučena.", ValidFrom = DateTime.UtcNow.AddDays(5), ValidTo = DateTime.UtcNow.AddDays(5).Date.AddHours(23), IsActive = true },
                new() { Title = "Pivní degustace", Description = "Ochutnávka speciálních craft piv z regionálních minipivovarů. Kapacita omezena.", ValidFrom = DateTime.UtcNow.AddDays(12), ValidTo = DateTime.UtcNow.AddDays(12).Date.AddHours(23), IsActive = true },
                new() { Title = "Palačinkový víkend", Description = "Speciální nabídka sladkých i slaných palačinek. Celý víkend od soboty do neděle.", ValidFrom = DateTime.UtcNow.AddDays(19), ValidTo = DateTime.UtcNow.AddDays(20).Date.AddHours(21), IsActive = true },
            };
            dbContext.Events.AddRange(events);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded events");
        }

        // Seed gallery images (placeholder paths)
        if (!await dbContext.GalleryImages.AnyAsync())
        {
            var images = new List<GalleryImage>
            {
                new() { FileName = "interior-1.jpg", FilePath = "/images/gallery/interior-1.jpg", AltText = "Interiér bistra", SortOrder = 1, IsActive = true },
                new() { FileName = "interior-2.jpg", FilePath = "/images/gallery/interior-2.jpg", AltText = "Bar a čep", SortOrder = 2, IsActive = true },
                new() { FileName = "food-1.jpg", FilePath = "/images/gallery/food-1.jpg", AltText = "Naše speciality", SortOrder = 3, IsActive = true },
                new() { FileName = "food-2.jpg", FilePath = "/images/gallery/food-2.jpg", AltText = "Domácí palačinky", SortOrder = 4, IsActive = true },
                new() { FileName = "beer-1.jpg", FilePath = "/images/gallery/beer-1.jpg", AltText = "Craft piva", SortOrder = 5, IsActive = true },
                new() { FileName = "beer-2.jpg", FilePath = "/images/gallery/beer-2.jpg", AltText = "Pivní degustace", SortOrder = 6, IsActive = true },
                new() { FileName = "atmosphere-1.jpg", FilePath = "/images/gallery/atmosphere-1.jpg", AltText = "Atmosféra v bistru", SortOrder = 7, IsActive = true },
                new() { FileName = "atmosphere-2.jpg", FilePath = "/images/gallery/atmosphere-2.jpg", AltText = "Večerní posezení", SortOrder = 8, IsActive = true },
                new() { FileName = "event-1.jpg", FilePath = "/images/gallery/event-1.jpg", AltText = "Akce v bistru", SortOrder = 9, IsActive = true },
                new() { FileName = "terrace-1.jpg", FilePath = "/images/gallery/terrace-1.jpg", AltText = "Venkovní posezení", SortOrder = 10, IsActive = true },
                new() { FileName = "hookah-1.jpg", FilePath = "/images/gallery/hookah-1.jpg", AltText = "Vodní dýmky", SortOrder = 11, IsActive = true },
                new() { FileName = "team-1.jpg", FilePath = "/images/gallery/team-1.jpg", AltText = "Náš tým", SortOrder = 12, IsActive = true },
            };
            dbContext.GalleryImages.AddRange(images);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded gallery images");
        }

        // Seed hero slides
        if (!await dbContext.HeroSlides.AnyAsync())
        {
            var slides = new List<HeroSlide>
            {
                new() { ImagePath = "/images/hero/hero-1.jpg", Title = "Bistro Stars Hollow", SortOrder = 1, IsActive = true },
                new() { ImagePath = "/images/hero/hero-2.jpg", Title = "Craft pivo z celého Česka", LinkUrl = "/Beers/DraftBeers", SortOrder = 2, IsActive = true },
                new() { ImagePath = "/images/hero/hero-3.jpg", Title = "Domácí palačinky", LinkUrl = "/Menu#jidelni-listek", SortOrder = 3, IsActive = true },
            };
            dbContext.HeroSlides.AddRange(slides);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded hero slides");
        }

        // Seed content blocks
        if (!await dbContext.ContentBlocks.AnyAsync())
        {
            var blocks = new List<ContentBlock>
            {
                new() { Key = "about-story", Title = "Náš příběh", Language = "cs", Content = "Bistro Stars Hollow vzniklo z lásky k dobrému pivu, poctivému jídlu a touhy vytvořit místo, kam se budete chtít vracet. Otevřeli jsme ve Frenštátě pod Radhoštěm s jednoduchým cílem – nabídnout kvalitu, pohodu a přátelskou atmosféru." },
                new() { Key = "about-story-detail", Title = "Náš příběh – detail", Language = "cs", Content = "Od samého začátku klademe důraz na pečlivý výběr surovin. Naše craft piva pochází z malých i velkých českých pivovarů, které osobně navštěvujeme. Kuchyně staví na jednoduchých receptech s čerstvými ingrediencemi – od klasických jídel po naše proslulé slané a sladké palačinky." },
                new() { Key = "homepage-about", Title = "O Bistro Stars Hollow", Language = "cs", Content = "Vítejte v Bistro Stars Hollow – útulném bistru v samém srdci Frenštátu pod Radhoštěm. Nabízíme pečlivě vybraná craft piva z českých i zahraničních pivovarů, domácí kuchyni připravovanou z čerstvých surovin a naše oblíbené slané i sladké palačinky." },
                new() { Key = "homepage-about-detail", Title = "O Bistro Stars Hollow – pokračování", Language = "cs", Content = "K posezení si můžete vychutnat kvalitní čaje, vodní dýmky nebo jednoduše strávit příjemný večer v přátelské atmosféře. Ať už přijdete na oběd, na pivo po práci nebo na víkendový brunch – u nás se budete cítit jako doma." },
            };
            dbContext.ContentBlocks.AddRange(blocks);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded content blocks");
        }

        // Seed staff on duty (today's demo)
        if (!await dbContext.StaffOnDuty.AnyAsync())
        {
            var employee = await userManager.FindByEmailAsync("zamestnanec@bistrosh.cz");
            var admin = await userManager.FindByEmailAsync("spravce@bistrosh.cz");

            var staff = new List<StaffOnDuty>();
            if (employee != null)
            {
                staff.Add(new StaffOnDuty { UserId = employee.Id, DisplayName = "Jan Novák", IsPrimary = true, Date = DateTime.UtcNow.Date });
            }
            if (admin != null)
            {
                staff.Add(new StaffOnDuty { UserId = admin.Id, DisplayName = "Marie Správcová", IsPrimary = false, Date = DateTime.UtcNow.Date });
            }

            if (staff.Any())
            {
                dbContext.StaffOnDuty.AddRange(staff);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded staff on duty");
            }
        }
    }
}
