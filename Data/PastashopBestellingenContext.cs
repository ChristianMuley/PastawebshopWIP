using Microsoft.EntityFrameworkCore;
using Pastashop.Models;

namespace Pastashop.Data;

public class PastashopBestellingenContext : DbContext
{
    public PastashopBestellingenContext(
    DbContextOptions<PastashopBestellingenContext> options) : base(options)
    {
        
    }

    public DbSet<Bestelling> Bestellingen { get; set; } = default!;
    public DbSet<BestellingCounter> BestellingCounters { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var bestelling = modelBuilder.Entity<Bestelling>();
        bestelling.HasIndex(b => b.OrderNummer)
                                        .IsUnique();
        bestelling.Property(b => b.OrderNummer)
                                        .HasMaxLength(20);
        
        modelBuilder.Entity<BestellingCounter>()
            .HasIndex(c => new {c.Year, c.Month})
            .IsUnique();
        
    }
}