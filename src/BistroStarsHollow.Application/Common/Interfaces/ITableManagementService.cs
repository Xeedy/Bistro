using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;

namespace BistroStarsHollow.Application.Common.Interfaces;

public interface ITableManagementService
{
    Task<List<BistroTable>> GetAllTablesAsync();
    Task<BistroTable?> GetTableByIdAsync(Guid id);
    Task CreateTableAsync(BistroTable table);
    Task UpdateTableAsync(BistroTable table);
    Task DeleteTableAsync(Guid id);
    Task ToggleTableActiveAsync(Guid id);
    Task UpdateTablePositionsAsync(List<TablePositionDto> positions);
}
