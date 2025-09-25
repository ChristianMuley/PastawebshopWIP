using Microsoft.EntityFrameworkCore;                     
using Pastashop.Models;                                 

namespace Pastashop.Data;

public class PastashopBestellingenContext : DbContext // Your EF Core DbContext = gateway to the database
{
    public PastashopBestellingenContext(
        DbContextOptions<PastashopBestellingenContext> options) : base(options) {}
    
    public DbSet<Bestelling> Bestellingen { get; set; } = null!;          // Tabel Bestellingen
    public DbSet<BestellingCounter> BestellingCounters { get; set; } = null!; // Tabel BestellingCounters

    protected override void OnModelCreating(ModelBuilder modelBuilder)        
    {
        base.OnModelCreating(modelBuilder);                                   

        // Bestelling
        var b = modelBuilder.Entity<Bestelling>();
        b.HasKey(x => x.Id);                                      // Primary key = Id
        b.Property(x => x.Id)
            .ValueGeneratedOnAdd()                                            // Autogeneratie van ID 
            .UseIdentityColumn();                                             // gebruik IDENTITY(1,1) 
        b.HasIndex(x => x.OrderNummer).IsUnique();                            // Elk ordernummer moet uniek zijn
        b.Property(x => x.OrderNummer).HasMaxLength(20);

        // BestellingsRegel
        var r = modelBuilder.Entity<BestellingsRegel>(); 
        r.HasKey(x => x.Id);                                                        // Primary key = Id
        r.Property(x => x.Id)
            .ValueGeneratedOnAdd()                                                                  // Autogeneratie van ID 
            .UseIdentityColumn();                                             
        r.HasOne(x => x.Bestelling)                                                // MTO
        .WithMany(x => x.Regels)                                                       // OTM
            .HasForeignKey(x => x.BestellingId)                                   // FK regels is BestellingId
            .OnDelete(DeleteBehavior.Cascade);                                                      // Verwijderen van bestelling verwijderd de regels erin

        // BestellingCounter
        var c = modelBuilder.Entity<BestellingCounter>();                    
        c.HasKey(x => x.Id);                                                  
        c.Property(x => x.Id)
            .ValueGeneratedOnAdd()                                            
            .UseIdentityColumn();                                            
        c.HasIndex(x => new { x.Year, x.Month }).IsUnique();                  // 1 counter per jaar/maand

    }

    public DbSet<BestellingsRegel> BestellingsRegels { get; set; } = null!;
}
