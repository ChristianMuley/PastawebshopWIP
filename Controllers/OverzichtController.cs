using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pastashop.Data;
using Pastashop.Models;

namespace Pastashop.Controllers;

public class OverzichtController : Controller
{
    private readonly PastashopBestellingenContext _db; //Bestellingen db
    public OverzichtController(PastashopBestellingenContext db) => _db = db; //DI van DbContext, opslaan in _db

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders = await _db.Bestellingen
            .Include(b => b.Regels)
            .OrderByDescending(b => b.Datum)
            .ToListAsync();

        return View(orders); // Views/Overzicht/Index.cshtml
    }

    
    // Markeer order als geleverd
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAfgeleverd(int id)
    {
        var b = await _db.Bestellingen.FindAsync(id);
        if (b == null) return NotFound();

        b.Afgeleverd = true;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
       
    }
    
    // Ongedaan maken..
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UndoMarkAfgeleverd(int id)
    {
        var b = await _db.Bestellingen.FindAsync(id);
        if (b ==null) return NotFound();

        b.Afgeleverd = false;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
        
    }
}