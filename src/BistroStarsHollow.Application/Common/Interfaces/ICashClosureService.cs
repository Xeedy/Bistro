using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface ICashClosureService
{
    Task<List<CashClosure>> GetAllAsync();
    Task<List<CashClosure>> GetByDateRangeAsync(DateTime? from, DateTime? to);
    Task<CashClosure?> GetByIdAsync(Guid id);
    Task<CashClosure?> GetByDateAsync(DateTime date);
    Task CreateAsync(CashClosure cashClosure);
    Task UpdateAsync(CashClosure cashClosure);
    Task DeleteAsync(Guid id);
}
