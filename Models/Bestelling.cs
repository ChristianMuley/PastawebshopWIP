namespace Pastashop.Models;

public class Bestelling
{
    public int Id { get; set; }

    public string OrderNummer { get; set; } = ""; 

    public string Product { get; set; } = "";
    
    public int Aantal { get; set; }

    public string Klant { get; set; } = "";
    
    public string? Kommentaar { get; set; } //Optioneel

    public DateTime Datum { get; set; } = DateTime.UtcNow; //default op nu
    
    public string SessionId { get; set; } //Om later meerdere bestellingen in eenzelfde sessie te mergen

}