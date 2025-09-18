using Microsoft.EntityFrameworkCore;
using Pastashop.Data;

namespace Pastashop.Models;

public class OrderNummerGenerator : IOrderNummerGenerator
{
    private readonly PastashopBestellingenContext _db;
    public OrderNummerGenerator(PastashopBestellingenContext db) =>  _db = db;

    public async Task<string> GenerateAsync()
    {
        var now = DateTime.UtcNow;
        var year = now.Year;
        var month = now.Month;

        await using var tx = await _db.Database.BeginTransactionAsync();

        var counter = await _db.BestellingCounters
            .SingleOrDefaultAsync(c => c.Year == year && c.Month == month);

        if (counter is null)
        {
            counter = new BestellingCounter { Year=year, Month=month, NextValue = 1 };
            _db.BestellingCounters.Add(counter);
            await _db.SaveChangesAsync();
        }

        var seq = counter.NextValue;
        counter.NextValue++;
        await _db.SaveChangesAsync();
        
        await tx.CommitAsync();

        var seq36 = ToBase36(seq).PadLeft(4, '0').ToUpperInvariant();


        var yy = (year % 100).ToString("D2");
        var mm = month.ToString("D2");
        return $"{seq36}-{yy}-{mm}";
    }

    private static string ToBase36(int value)
    {
        if (value == 0) return "0";
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var chars = new Stack<char>();
        var v = value;
        while (v > 0)
        {
            var rem = v % 36;
            chars.Push(alphabet[rem]);
            v /= 36;
        }
        return new string(chars.ToArray());
    }
}