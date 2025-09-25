using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pastashop.Models;

public class BestellingsRegel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int BestellingId { get; set; }
    public Bestelling Bestelling { get; set; } = null!;

    public PastaSoort Pasta { get; set; }
    public PortieGrootte Grootte { get; set; }
    public SausType Saus { get; set; }

    public int Aantal { get; set; } = 1;
}