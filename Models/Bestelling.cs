using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pastashop.Models;

public class Bestelling
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string OrderNummer { get; set; } = ""; 

    public string Product { get; set; } = "";
    
    public int Aantal { get; set; }

    public string Klant { get; set; } = "";
    
    public string? Kommentaar { get; set; } //Optioneel

    public DateTime Datum { get; set; } = DateTime.UtcNow; //default op nu
    
    public string? SessionId { get; set; } //Om later meerdere bestellingen in eenzelfde sessie te mergen
    
    public ICollection<BestellingsRegel> Regels { get; set; } = new List<BestellingsRegel>();

    public bool Afgeleverd { get; set; } = false;

}