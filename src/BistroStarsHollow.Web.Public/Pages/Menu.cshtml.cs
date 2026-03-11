using BistroStarsHollow.Application.Common.Interfaces;
using BistroStarsHollow.Application.Common.Models;
using BistroStarsHollow.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BistroStarsHollow.Web.Public.Pages;

public class MenuModel : PageModel
{
    private readonly IPublicDataService _publicData;

    public MenuModel(IPublicDataService publicData)
    {
        _publicData = publicData;
    }

    public List<MenuItemDto> FoodItems { get; set; } = new();
    public List<MenuItemDto> DrinkItems { get; set; } = new();
    public List<MenuItemDto> WaterPipeItems { get; set; } = new();
    public List<MenuItemDto> TeaItems { get; set; } = new();
    public List<MenuItemDto> ProductItems { get; set; } = new();

    public async Task OnGetAsync()
    {
        var grouped = await _publicData.GetAllMenuItemsGroupedAsync();

        FoodItems = grouped.GetValueOrDefault(MenuCategory.Food, new());
        DrinkItems = grouped.GetValueOrDefault(MenuCategory.ColdDrinks, new());
        WaterPipeItems = grouped.GetValueOrDefault(MenuCategory.WaterPipes, new());
        TeaItems = grouped.GetValueOrDefault(MenuCategory.Tea, new());
        ProductItems = grouped.GetValueOrDefault(MenuCategory.Products, new());
    }
}
