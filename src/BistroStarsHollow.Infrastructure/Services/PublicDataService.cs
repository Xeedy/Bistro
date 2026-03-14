using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;
using BistroStarsHollow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BistroStarsHollow.Infrastructure.Services;

public class PublicDataService : IPublicDataService
{
    private readonly ApplicationDbContext _dbContext;

    public PublicDataService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MenuItemDto>> GetMenuItemsByCategoryAsync(MenuCategory category)
    {
        return await _dbContext.MenuItems
            .Where(m => m.IsActive && m.Category == category)
            .OrderBy(m => m.SortOrder)
            .Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Category = m.Category,
                Price = m.Price,
                DisplayPrice = m.DisplayPrice,
                Description = m.Description,
                SortOrder = m.SortOrder
            })
            .ToListAsync();
    }

    public async Task<Dictionary<MenuCategory, List<MenuItemDto>>> GetAllMenuItemsGroupedAsync()
    {
        var items = await _dbContext.MenuItems
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Category = m.Category,
                Price = m.Price,
                DisplayPrice = m.DisplayPrice,
                Description = m.Description,
                SortOrder = m.SortOrder
            })
            .ToListAsync();

        return items.GroupBy(i => i.Category)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<List<BeerDto>> GetActiveBeersByTypeAsync(BeerType type)
    {
        return await _dbContext.Beers
            .Include(b => b.Brewery)
            .Where(b => b.IsActive && b.Type == type)
            .OrderBy(b => b.SortOrder)
            .Select(b => new BeerDto
            {
                Id = b.Id,
                Name = b.Name,
                Type = b.Type,
                Style = b.Style,
                Capacity = b.Capacity,
                Price = b.Price,
                PriceSmall = b.PriceSmall,
                Description = b.Description,
                ImagePath = b.ImagePath,
                SortOrder = b.SortOrder,
                BreweryId = b.BreweryId,
                BreweryName = b.Brewery.Name,
                BreweryLogoPath = b.Brewery.LogoImagePath
            })
            .ToListAsync();
    }

    public async Task<List<BreweryDto>> GetActiveBreweriesAsync()
    {
        return await _dbContext.Breweries
            .Where(b => b.IsActive)
            .OrderBy(b => b.SortOrder)
            .Select(b => new BreweryDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoImagePath = b.LogoImagePath,
                SortOrder = b.SortOrder,
                BeerCount = b.Beers.Count(beer => beer.IsActive)
            })
            .ToListAsync();
    }

    public async Task<BreweryDto?> GetBreweryByIdAsync(Guid id)
    {
        return await _dbContext.Breweries
            .Where(b => b.Id == id && b.IsActive)
            .Select(b => new BreweryDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoImagePath = b.LogoImagePath,
                SortOrder = b.SortOrder,
                BeerCount = b.Beers.Count(beer => beer.IsActive),
                Beers = b.Beers
                    .Where(beer => beer.IsActive)
                    .OrderBy(beer => beer.SortOrder)
                    .Select(beer => new BeerDto
                    {
                        Id = beer.Id,
                        Name = beer.Name,
                        Type = beer.Type,
                        Style = beer.Style,
                        Capacity = beer.Capacity,
                        Price = beer.Price,
                        PriceSmall = beer.PriceSmall,
                        Description = beer.Description,
                        ImagePath = beer.ImagePath,
                        SortOrder = beer.SortOrder,
                        BreweryId = beer.BreweryId,
                        BreweryName = b.Name,
                        BreweryLogoPath = b.LogoImagePath
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<OpeningHours>> GetOpeningHoursAsync()
    {
        return await _dbContext.OpeningHours
            .OrderBy(o => o.DayOfWeek)
            .ToListAsync();
    }

    public async Task<List<Event>> GetActiveEventsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Events
            .Where(e => e.IsActive && (e.ValidTo == null || e.ValidTo >= now))
            .OrderBy(e => e.ValidFrom)
            .ToListAsync();
    }

    public async Task<List<GalleryImage>> GetActiveGalleryImagesAsync()
    {
        return await _dbContext.GalleryImages
            .Where(g => g.IsActive)
            .OrderBy(g => g.SortOrder)
            .ToListAsync();
    }

    public async Task<List<HeroSlide>> GetActiveHeroSlidesAsync()
    {
        return await _dbContext.HeroSlides
            .Where(h => h.IsActive)
            .OrderBy(h => h.SortOrder)
            .ToListAsync();
    }

    public async Task<ContentBlock?> GetContentBlockAsync(string key, string language = "cs")
    {
        return await _dbContext.ContentBlocks
            .FirstOrDefaultAsync(c => c.Key == key && c.Language == language);
    }

    public async Task<List<StaffOnDuty>> GetStaffOnDutyAsync(DateTime date)
    {
        return await _dbContext.StaffOnDuty
            .Where(s => s.Date == date.Date)
            .OrderByDescending(s => s.IsPrimary)
            .ToListAsync();
    }
}
