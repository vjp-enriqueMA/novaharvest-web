using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NovaHarvest.Web.Pages.Reservations;

public class TicketModel : PageModel
{
    public string Reservation { get; private set; } = "";
    public string Product { get; private set; } = "";
    public int Kg { get; private set; }
    public string Pickup { get; private set; } = "";
    public DateTime GeneratedAt { get; private set; }

    public void OnGet()
    {
        Reservation = "RES-0001";
        Product = "Cereza Picota";
        Kg = 20;
        Pickup = "Cooperativa del Valle del Jerte";
        GeneratedAt = DateTime.Now;
    }
}