using System.ComponentModel.DataAnnotations;

namespace Pastashop.Models;

public class BestelItem
{
    public PastaSoort Pasta {get; set;}
    public PortieGrootte Grootte  {get; set;}
    public SausType Saus {get; set;}
    
    [Range(1,20, ErrorMessage ="Aantal moet tussen 1 en 20 zijn.")]
    public int Aantal { get; set; } = 1;
}
