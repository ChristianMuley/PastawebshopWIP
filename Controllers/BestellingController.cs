using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Pastashop.Data;              
using Pastashop.Models;           
using Microsoft.EntityFrameworkCore; 
using System.Linq;

namespace Pastashop.Controllers
{
    public class BestellingController : Controller
    {
        private readonly PastashopBestellingenContext _db;
        private readonly IOrderNummerGenerator _gen;

        public BestellingController(PastashopBestellingenContext db, IOrderNummerGenerator gen)
        {
            _db  = db; //database
            _gen = gen; //seq36 ordernummer generator
        }

        // GET: Winkelmandje (start)
        [HttpGet]
        public IActionResult Index()
        {
            return View(new BestelItem());
        }

        // POST: Winkelmandje (item toevoegen)
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF protection
        public IActionResult Toevoegen(BestelItem item)
        {
            // lees cart uit session + geen double-submit bij refresh
            // uit session zodat DB niet belaagd wordt met lege carts, zo blijft het ook per user per browser
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data)
                ? new List<BestelItem>() // als leeg -> nieuwe lege cart
                : JsonSerializer.Deserialize<List<BestelItem>>(data)!;

            // voeg toe
            cart.Add(item);

            // schrijf terug naar session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));


            TempData["msg"] = "Toegevoegd aan je mandje!";
            return RedirectToAction("Index", "Home");
        }

        // GET: Winkelmandje (overzicht)
        [HttpGet]
        public IActionResult Mandje()
        {
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data)
                ? new List<BestelItem>()
                : JsonSerializer.Deserialize<List<BestelItem>>(data)!;

            return View(cart);
        }

        // POST: Bestelling doorvoeren (opslaan in DB + mandje leegmaken)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BestellingDoorvoeren()
        { 
            // Mandje van sessie laden
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data) 
            ? new List<BestelItem>()
            : JsonSerializer.Deserialize<List<BestelItem>>(data)!;
        
       
        // Geen lege bestellingen!!
        if (!cart.Any())
        {
            TempData["Bedankt"] = "Je mandje is leeg.";
            return RedirectToAction(nameof(Mandje));
        }

    // groepeer gelijke items en tel de aantallen
    // Werkt nog niet!!
    var grouped = cart
        .GroupBy(i => new { i.Pasta, i.Grootte, i.Saus })
        .Select(g => new { g.Key.Pasta, g.Key.Grootte, g.Key.Saus, Qty = g.Sum(x => x.Aantal) })
        .ToList();

    // Veranderd elke groep naar string: "Product Grootte Additive Aantal" / Bv "Spaghetti Klein met Pesto x3"
    var productList  = string.Join(", ", grouped.Select(g => $"{g.Pasta} {g.Grootte} met {g.Saus} ×{g.Qty}"));
    var totaalAantal = grouped.Sum(g => g.Qty);

    // maak de bestelling
    var bestelling = new Bestelling
    {
        OrderNummer = await _gen.GenerateAsync(), //haalt de volgende gegenereerde Ordernummer
        Product     = productList,
        Aantal      = totaalAantal,
        Klant       = User?.Identity?.Name ?? "Onbekend",
        Kommentaar  = null,
        Datum       = DateTime.UtcNow,
        SessionId   = HttpContext.Session.Id
    };

    // voeg 1 regel per cart-item toe  
    foreach (var i in cart)
    {
        bestelling.Regels.Add(new BestellingsRegel
        {
            Pasta   = i.Pasta,
            Grootte = i.Grootte,
            Saus    = i.Saus,
            Aantal  = i.Aantal
        });
    }

        // Circumventie van 'IDENTITY_INSERT is OFF' fout.
        // EF behandeld alles als "nieuw"
        bestelling.Id = 0;
        foreach (var r in bestelling.Regels) { r.Id = 0; r.BestellingId = 0; }

        // voeg bestelling (met regels) één keer toe en sla één keer op
        _db.Bestellingen.Add(bestelling);

        // Wat als er 2 requests tergelijk komen voor eenzelfde ordernummer?
        await _db.SaveChangesAsync();
    
    
  

    // Succes: Verwijderd cart + dank u
    HttpContext.Session.Remove("Cart");
    TempData["Bedankt"] = $"Bedankt voor je bestelling!  Order #{bestelling.OrderNummer}";
    return RedirectToAction(nameof(Mandje));
    
    
    }


    // GET: Nieuwsbrief
    [HttpGet]
    public IActionResult Nieuwsbrief() => View();

    // POST: Nieuwsbriefbevestiging
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Nieuwsbrief(NieuwsbriefModel model)
    {
        if (!ModelState.IsValid) return View(model);
        return View("NieuwsbriefBevestiging", model);
    }
    }
}
