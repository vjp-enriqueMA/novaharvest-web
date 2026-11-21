namespace NovaHarvest.Web.Models;

public class Surplus
{
    public string Product { get; set; } = "";
    public string Producer { get; set; } = "";
    public string Village { get; set; } = "";
    public DateTime HarvestDate { get; set; }
    public int AvailableKg { get; set; }
    public int ReservedKg { get; set; }
    public decimal PricePerKg { get; set; }
    public string? Note { get; set; }
}