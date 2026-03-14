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
    Task ToggleSlideActiveAsync(Guid id);
    Task UpdateSlideSortOrderAsync(List<Guid> orderedIds);

    // Gallery
    Task<List<GalleryImage>> GetAllGalleryImagesAsync();
    Task<GalleryImage?> GetGalleryImageByIdAsync(Guid id);
    Task CreateGalleryImageAsync(GalleryImage image);
    Task UpdateGalleryImageAsync(GalleryImage image);
    Task DeleteGalleryImageAsync(Guid id);
    Task ToggleGalleryImageActiveAsync(Guid id);
    Task UpdateGalleryImageSortOrderAsync(List<Guid> orderedIds);

    // Events
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(Guid id);
    Task CreateEventAsync(Event evt);
    Task UpdateEventAsync(Event evt);
    Task DeleteEventAsync(Guid id);
    Task ToggleEventActiveAsync(Guid id);

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
    Task ToggleMenuItemActiveAsync(Guid id);
    Task UpdateMenuItemSortOrderAsync(List<Guid> orderedIds);

    // Beers
    Task<List<Beer>> GetAllBeersAsync();
    Task<Beer?> GetBeerByIdAsync(Guid id);
    Task CreateBeerAsync(Beer beer);
    Task UpdateBeerAsync(Beer beer);
    Task DeleteBeerAsync(Guid id);
    Task ToggleBeerActiveAsync(Guid id);
    Task<int> GetActiveDraftBeerCountAsync();
    Task UpdateBeerSortOrderAsync(List<Guid> orderedIds);

    // Beer Styles
    Task<List<BeerStyle>> GetAllBeerStylesAsync();
    Task CreateBeerStyleAsync(BeerStyle style);
    Task UpdateBeerStyleAsync(BeerStyle style);
    Task DeleteBeerStyleAsync(Guid id);
    Task UpdateBeerStyleSortOrderAsync(List<Guid> orderedIds);

    // Breweries
    Task<List<Brewery>> GetAllBreweriesAsync();
    Task<Brewery?> GetBreweryByIdAsync(Guid id);
    Task CreateBreweryAsync(Brewery brewery);
    Task UpdateBreweryAsync(Brewery brewery);
    Task DeleteBreweryAsync(Guid id);
    Task<int> GetBreweryBeerCountAsync(Guid breweryId);
    Task ToggleBreweryActiveAsync(Guid id);
    Task UpdateBrewerySortOrderAsync(List<Guid> orderedIds);
}
