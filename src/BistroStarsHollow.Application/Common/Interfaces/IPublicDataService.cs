using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Domain.Enums;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IPublicDataService
{
    // Menu
    Task<List<MenuItemDto>> GetMenuItemsByCategoryAsync(MenuCategory category);
    Task<Dictionary<MenuCategory, List<MenuItemDto>>> GetAllMenuItemsGroupedAsync();

    // Beers
    Task<List<BeerDto>> GetActiveBeersByTypeAsync(BeerType type);

    // Breweries
    Task<List<BreweryDto>> GetActiveBreweriesAsync();
    Task<BreweryDto?> GetBreweryByIdAsync(Guid id);

    // Opening Hours
    Task<List<OpeningHours>> GetOpeningHoursAsync();

    // Events
    Task<List<Event>> GetActiveEventsAsync();

    // Gallery
    Task<List<GalleryImage>> GetActiveGalleryImagesAsync();

    // Hero Slides
    Task<List<HeroSlide>> GetActiveHeroSlidesAsync();

    // Content Blocks
    Task<ContentBlock?> GetContentBlockAsync(string key, string language = "cs");

    // Staff On Duty
    Task<List<StaffOnDuty>> GetStaffOnDutyAsync(DateTime date);
}
