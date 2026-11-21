using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaHarvest.Web.Models;

namespace NovaHarvest.Web.Pages;

public class SurplusModel : PageModel
{
    private const decimal SurplusDiscount = 0.30m;
    private const int BoxKg = 5;
    private const int ShelfLifeDays = 7;
    private const int LowStockKg = 50;

    public Surplus Surplus { get; } = new()
    {
        Product = "Cereza Picota",
        Producer = "Huerta Los Castaños",
        Village = "Cabezuela del Valle",
        HarvestDate = DateTime.Today.AddDays(-2),
        AvailableKg = 343,
        ReservedKg = 120,
        PricePerKg = 3.20m,
        Note = "<b>Recoger antes de las 14:00</b>"
    };

    // DESDE AQUÍ HASTA ABAJO ME HE AYUDADO DE LA INTELIGENCIA ARTIFICIAL
    public int RemainingKg { get; private set; }
    public int FullBoxes { get; private set; }
    public int LooseKg { get; private set; }
    public decimal DiscountedPricePerKg { get; private set; }
    public decimal TotalValue { get; private set; }
    public DateTime ConsumeBefore { get; private set; }
    public int DaysToConsume { get; private set; }
    public string Status { get; private set; } = "";
    public bool IsOutOfStock { get; private set; }
    public bool IsUrgent { get; private set; }

    public void OnGet()
    {
        RemainingKg = Surplus.AvailableKg - Surplus.ReservedKg;

        FullBoxes = RemainingKg / BoxKg;
        LooseKg = RemainingKg % BoxKg;

        DiscountedPricePerKg = Surplus.PricePerKg * (1 - SurplusDiscount);

        TotalValue = RemainingKg * DiscountedPricePerKg;

        ConsumeBefore = Surplus.HarvestDate.AddDays(ShelfLifeDays);

        DaysToConsume = (ConsumeBefore - DateTime.Today).Days;

        Status = RemainingKg == 0
            ? "Agotado"
            : (RemainingKg < LowStockKg ? "¡Últimos kilos!" : "Disponible");

        IsOutOfStock = RemainingKg == 0;

        IsUrgent = DaysToConsume <= 2 && RemainingKg > 0;
    }
}