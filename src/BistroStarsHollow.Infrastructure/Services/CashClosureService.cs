using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BistroStarsHollow.Infrastructure.Services;

public class CashClosureService : ICashClosureService
{
    private readonly ApplicationDbContext _context;

    public CashClosureService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CashClosure>> GetAllAsync()
        => await _context.CashClosures.OrderByDescending(c => c.Date).ToListAsync();

    public async Task<List<CashClosure>> GetByDateRangeAsync(DateTime? from, DateTime? to)
    {
        var query = _context.CashClosures.AsQueryable();

        if (from.HasValue)
            query = query.Where(c => c.Date >= from.Value.Date);

        if (to.HasValue)
            query = query.Where(c => c.Date <= to.Value.Date);

        return await query.OrderByDescending(c => c.Date).ToListAsync();
    }

    public async Task<CashClosure?> GetByIdAsync(Guid id)
        => await _context.CashClosures.FindAsync(id);

    public async Task<CashClosure?> GetByDateAsync(DateTime date)
        => await _context.CashClosures.FirstOrDefaultAsync(c => c.Date == date.Date);

    public async Task CreateAsync(CashClosure cashClosure)
    {
        cashClosure.TotalAmount = cashClosure.CardAmount + cashClosure.CashAmount;
        _context.CashClosures.Add(cashClosure);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CashClosure cashClosure)
    {
        cashClosure.TotalAmount = cashClosure.CardAmount + cashClosure.CashAmount;
        _context.CashClosures.Update(cashClosure);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var closure = await _context.CashClosures.FindAsync(id);
        if (closure != null)
        {
            _context.CashClosures.Remove(closure);
            await _context.SaveChangesAsync();
        }
    }
}
