using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pastashop.Models;

public class BestellingCounter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 
    public int Year { get; set; }
    public int Month { get; set; }
    public int NextValue { get; set; }

    
}