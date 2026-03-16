using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Entities;
using BistroStarsHollow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BistroStarsHollow.Infrastructure.Services;

public class TableManagementService : ITableManagementService
{
    private readonly ApplicationDbContext _dbContext;

    public TableManagementService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BistroTable>> GetAllTablesAsync()
    {
        return await _dbContext.BistroTables
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }

    public async Task<BistroTable?> GetTableByIdAsync(Guid id)
    {
        return await _dbContext.BistroTables.FindAsync(id);
    }

    public async Task CreateTableAsync(BistroTable table)
    {
        _dbContext.BistroTables.Add(table);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTableAsync(BistroTable table)
    {
        var existing = await _dbContext.BistroTables.FindAsync(table.Id);
        if (existing == null) return;

        existing.Name = table.Name;
        existing.Capacity = table.Capacity;
        existing.SortOrder = table.SortOrder;
        existing.IsActive = table.IsActive;
        existing.MapX = table.MapX;
        existing.MapY = table.MapY;
        existing.Zone = table.Zone;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTableAsync(Guid id)
    {
        var table = await _dbContext.BistroTables.FindAsync(id);
        if (table == null) return;

        _dbContext.BistroTables.Remove(table);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ToggleTableActiveAsync(Guid id)
    {
        var table = await _dbContext.BistroTables.FindAsync(id);
        if (table == null) return;

        table.IsActive = !table.IsActive;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTablePositionsAsync(List<TablePositionDto> positions)
    {
        foreach (var pos in positions)
        {
            var table = await _dbContext.BistroTables.FindAsync(pos.Id);
            if (table != null)
            {
                table.MapX = pos.MapX;
                table.MapY = pos.MapY;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
