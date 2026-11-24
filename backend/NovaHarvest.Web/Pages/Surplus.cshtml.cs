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
        // Calculo los kilos que quedan después de las reservas.
        RemainingKg = Surplus.AvailableKg - Surplus.ReservedKg;

        // Calculo cuántas cajas completas se pueden preparar y los kilos que sobran.
        FullBoxes = RemainingKg / BoxKg;
        LooseKg = RemainingKg % BoxKg;

        // Calculo el precio por kilo aplicando el descuento del 30%.
        DiscountedPricePerKg = Surplus.PricePerKg * (1 - SurplusDiscount);

        // Calculo el valor total de los kilos que quedan.
        TotalValue = RemainingKg * DiscountedPricePerKg;

        // Calculo la fecha límite para consumir el excedente.
        ConsumeBefore = Surplus.HarvestDate.AddDays(ShelfLifeDays);

        // Calculo cuántos días quedan hasta esa fecha.
        DaysToConsume = (ConsumeBefore - DateTime.Today).Days;

        // Compruebo el estado del excedente según los kilos disponibles.
        Status = RemainingKg == 0
            ? "Agotado"
            : (RemainingKg < LowStockKg ? "¡Últimos kilos!" : "Disponible");

        // Compruebo si ya no quedan kilos disponibles.
        IsOutOfStock = RemainingKg == 0;

        // Compruebo si quedan dos días o menos y todavía hay kilos.
        IsUrgent = DaysToConsume <= 2 && RemainingKg > 0;
    }
}