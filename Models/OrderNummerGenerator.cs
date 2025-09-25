using Microsoft.EntityFrameworkCore;
using Pastashop.Data;

namespace Pastashop.Models;

public class OrderNummerGenerator : IOrderNummerGenerator
{
    private readonly PastashopBestellingenContext _db;
    public OrderNummerGenerator(PastashopBestellingenContext db) => _db = db;

    // Maakt een nieuw order nummer zoals "00AB-25-09"
    public async Task<string> GenerateAsync()
    {
        var now   = DateTime.UtcNow; // huidige datum/tijd
        var year  = now.Year; // jaar
        var month = now.Month; // maand

        // Laat EF retries handelen als er iets mis gaat met de db
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            // start transactie zodat niets half-opslaat
            await using var tx = await _db.Database.BeginTransactionAsync();

            // Kijken of we al een row hebben voor dit jaar+maand
            var counter = await _db.BestellingCounters
                .Where(c => c.Year == year && c.Month == month)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

            // Als er gene bestaat, maak een nieuwe die start bij 1
            if (counter is null)
            {
                counter = new BestellingCounter { Year = year, Month = month, NextValue = 1 };
                _db.BestellingCounters.Add(counter);
                try
                {
                    await _db.SaveChangesAsync(); // probeer opslaan?
                }
                catch (DbUpdateException) // als iemand anders al een order geadd heeft? / nog te testen
                {
                    _db.Entry(counter).State = EntityState.Detached;
                    counter = await _db.BestellingCounters
                        .Where(c => c.Year == year && c.Month == month)
                        .OrderBy(c => c.Id)
                        .FirstAsync();
                }
            }

            // Neem huidige nummer, verhoog & stuur naar db
            var seq = counter.NextValue;
            counter.NextValue++;
            await _db.SaveChangesAsync();

            // Beeïndig transactie
            await tx.CommitAsync();

            // Bouw de ordernummer string;  Formaat: SEQ36-YY-MM
            var seq36 = ToBase36(seq).PadLeft(4, '0').ToUpperInvariant();
            var yy = (year % 100).ToString("D2");
            var mm = month.ToString("D2");
            return $"{seq36}-{yy}-{mm}";
        });
    }

    // Veranderd getal naar base-36 (0-9 dan A-Z)
    private static string ToBase36(int value)
    {
        if (value == 0) return "0";
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var chars = new Stack<char>();
        for (int v = value; v > 0; v /= 36) chars.Push(alphabet[v % 36]);
        return new string(chars.ToArray());
    }
}
