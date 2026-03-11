using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface IContentManagementService
{
    // Hero Slides
    Task<List<HeroSlide>> GetAllSlidesAsync();
    Task<HeroSlide?> GetSlideByIdAsync(Guid id);
    Task CreateSlideAsync(HeroSlide slide);
    Task UpdateSlideAsync(HeroSlide slide);
    Task DeleteSlideAsync(Guid id);

    // Gallery
    Task<List<GalleryImage>> GetAllGalleryImagesAsync();
    Task<GalleryImage?> GetGalleryImageByIdAsync(Guid id);
    Task CreateGalleryImageAsync(GalleryImage image);
    Task UpdateGalleryImageAsync(GalleryImage image);
    Task DeleteGalleryImageAsync(Guid id);

    // Events
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(Guid id);
    Task CreateEventAsync(Event evt);
    Task UpdateEventAsync(Event evt);
    Task DeleteEventAsync(Guid id);

    // Opening Hours
    Task<List<OpeningHours>> GetAllOpeningHoursAsync();
    Task UpdateOpeningHoursAsync(List<OpeningHours> hours);

    // Content Blocks
    Task<List<ContentBlock>> GetAllContentBlocksAsync();
    Task<ContentBlock?> GetContentBlockByIdAsync(Guid id);
    Task CreateContentBlockAsync(ContentBlock block);
    Task UpdateContentBlockAsync(ContentBlock block);
    Task DeleteContentBlockAsync(Guid id);

    // Menu Items
    Task<List<MenuItem>> GetAllMenuItemsAsync();
    Task<MenuItem?> GetMenuItemByIdAsync(Guid id);
    Task CreateMenuItemAsync(MenuItem item);
    Task UpdateMenuItemAsync(MenuItem item);
    Task DeleteMenuItemAsync(Guid id);

    // Beers
    Task<List<Beer>> GetAllBeersAsync();
    Task<Beer?> GetBeerByIdAsync(Guid id);
    Task CreateBeerAsync(Beer beer);
    Task UpdateBeerAsync(Beer beer);
    Task DeleteBeerAsync(Guid id);

    // Breweries
    Task<List<Brewery>> GetAllBreweriesAsync();
    Task<Brewery?> GetBreweryByIdAsync(Guid id);
    Task CreateBreweryAsync(Brewery brewery);
    Task UpdateBreweryAsync(Brewery brewery);
    Task DeleteBreweryAsync(Guid id);
    Task<int> GetBreweryBeerCountAsync(Guid breweryId);
}
