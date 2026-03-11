using Microsoft.EntityFrameworkCore;
using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Infrastructure.Data;

namespace BistroStarsHollow.Infrastructure.Services;

public class ContentManagementService : IContentManagementService
{
    private readonly ApplicationDbContext _context;

    public ContentManagementService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Hero Slides
    public async Task<List<HeroSlide>> GetAllSlidesAsync()
        => await _context.HeroSlides.OrderBy(s => s.SortOrder).ToListAsync();

    public async Task<HeroSlide?> GetSlideByIdAsync(Guid id)
        => await _context.HeroSlides.FindAsync(id);

    public async Task CreateSlideAsync(HeroSlide slide)
    {
        _context.HeroSlides.Add(slide);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSlideAsync(HeroSlide slide)
    {
        _context.HeroSlides.Update(slide);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSlideAsync(Guid id)
    {
        var slide = await _context.HeroSlides.FindAsync(id);
        if (slide != null)
        {
            _context.HeroSlides.Remove(slide);
            await _context.SaveChangesAsync();
        }
    }

    // Gallery
    public async Task<List<GalleryImage>> GetAllGalleryImagesAsync()
        => await _context.GalleryImages.OrderBy(g => g.SortOrder).ToListAsync();

    public async Task<GalleryImage?> GetGalleryImageByIdAsync(Guid id)
        => await _context.GalleryImages.FindAsync(id);

    public async Task CreateGalleryImageAsync(GalleryImage image)
    {
        _context.GalleryImages.Add(image);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateGalleryImageAsync(GalleryImage image)
    {
        _context.GalleryImages.Update(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGalleryImageAsync(Guid id)
    {
        var image = await _context.GalleryImages.FindAsync(id);
        if (image != null)
        {
            _context.GalleryImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    // Events
    public async Task<List<Event>> GetAllEventsAsync()
        => await _context.Events.OrderByDescending(e => e.ValidFrom).ToListAsync();

    public async Task<Event?> GetEventByIdAsync(Guid id)
        => await _context.Events.FindAsync(id);

    public async Task CreateEventAsync(Event evt)
    {
        _context.Events.Add(evt);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(Event evt)
    {
        _context.Events.Update(evt);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt != null)
        {
            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
        }
    }

    // Opening Hours
    public async Task<List<OpeningHours>> GetAllOpeningHoursAsync()
        => await _context.OpeningHours.OrderBy(o => o.DayOfWeek).ToListAsync();

    public async Task UpdateOpeningHoursAsync(List<OpeningHours> hours)
    {
        foreach (var h in hours)
        {
            var existing = await _context.OpeningHours.FindAsync(h.Id);
            if (existing != null)
            {
                existing.OpenTime = h.OpenTime;
                existing.CloseTime = h.CloseTime;
                existing.IsClosed = h.IsClosed;
            }
        }
        await _context.SaveChangesAsync();
    }

    // Content Blocks
    public async Task<List<ContentBlock>> GetAllContentBlocksAsync()
        => await _context.ContentBlocks.OrderBy(c => c.Key).ThenBy(c => c.Language).ToListAsync();

    public async Task<ContentBlock?> GetContentBlockByIdAsync(Guid id)
        => await _context.ContentBlocks.FindAsync(id);

    public async Task CreateContentBlockAsync(ContentBlock block)
    {
        _context.ContentBlocks.Add(block);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateContentBlockAsync(ContentBlock block)
    {
        _context.ContentBlocks.Update(block);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteContentBlockAsync(Guid id)
    {
        var block = await _context.ContentBlocks.FindAsync(id);
        if (block != null)
        {
            _context.ContentBlocks.Remove(block);
            await _context.SaveChangesAsync();
        }
    }

    // Menu Items
    public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        => await _context.MenuItems.OrderBy(m => m.Category).ThenBy(m => m.SortOrder).ToListAsync();

    public async Task<MenuItem?> GetMenuItemByIdAsync(Guid id)
        => await _context.MenuItems.FindAsync(id);

    public async Task CreateMenuItemAsync(MenuItem item)
    {
        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMenuItemAsync(MenuItem item)
    {
        _context.MenuItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenuItemAsync(Guid id)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item != null)
        {
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    // Beers
    public async Task<List<Beer>> GetAllBeersAsync()
        => await _context.Beers.Include(b => b.Brewery).OrderBy(b => b.SortOrder).ToListAsync();

    public async Task<Beer?> GetBeerByIdAsync(Guid id)
        => await _context.Beers.Include(b => b.Brewery).FirstOrDefaultAsync(b => b.Id == id);

    public async Task CreateBeerAsync(Beer beer)
    {
        _context.Beers.Add(beer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBeerAsync(Beer beer)
    {
        _context.Beers.Update(beer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBeerAsync(Guid id)
    {
        var beer = await _context.Beers.FindAsync(id);
        if (beer != null)
        {
            _context.Beers.Remove(beer);
            await _context.SaveChangesAsync();
        }
    }

    // Breweries
    public async Task<List<Brewery>> GetAllBreweriesAsync()
        => await _context.Breweries.OrderBy(b => b.SortOrder).ToListAsync();

    public async Task<Brewery?> GetBreweryByIdAsync(Guid id)
        => await _context.Breweries.FindAsync(id);

    public async Task CreateBreweryAsync(Brewery brewery)
    {
        _context.Breweries.Add(brewery);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBreweryAsync(Brewery brewery)
    {
        _context.Breweries.Update(brewery);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBreweryAsync(Guid id)
    {
        var brewery = await _context.Breweries.FindAsync(id);
        if (brewery != null)
        {
            _context.Breweries.Remove(brewery);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetBreweryBeerCountAsync(Guid breweryId)
        => await _context.Beers.CountAsync(b => b.BreweryId == breweryId);
}
