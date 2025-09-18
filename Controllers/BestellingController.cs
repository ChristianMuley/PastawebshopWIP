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
            _db  = db;
            _gen = gen;
        }

        // GET: Winkelmandje (start)
        [HttpGet]
        public IActionResult Index()
        {
            return View(new BestelItem());
        }

        // POST: Winkelmandje (item toevoegen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toevoegen(BestelItem item)
        {
            // lees cart uit session
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data)
                ? new List<BestelItem>()
                : JsonSerializer.Deserialize<List<BestelItem>>(data)!;

            // voeg toe
            cart.Add(item);

            // schrijf terug naar session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

            return RedirectToAction(nameof(Mandje));
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
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data)
                ? new List<BestelItem>()
                : JsonSerializer.Deserialize<List<BestelItem>>(data)!;

            if (!cart.Any())
            {
                TempData["Bedankt"] = "Je mandje is leeg.";
                return RedirectToAction(nameof(Mandje));
            }

            // maak samenvatting per item (bijv. "Spaghetti Groot met Bolognese, Penne Klein met Pesto")
            var parts = new List<string>();
            foreach (var i in cart)
            {
                parts.Add($"{i.Pasta} {i.Grootte} met {i.Saus}");
            }
            var productList = string.Join(", ", parts);

          // totaal aantal items = aantal regels in het mandje
            var totaalAantal = cart.Count;

            // genereer  ordernummer (BASE36-YY-MM) en bouw entity
            var bestelling = new Bestelling
            {
                OrderNummer = await _gen.GenerateAsync(),
                Product     = productList,                    // eenvoudig samengevat
                Aantal      = totaalAantal,
                Klant       = User?.Identity?.Name ?? "Onbekend",
                Kommentaar  = null,
                Datum       = DateTime.UtcNow,
                SessionId   = HttpContext.Session.Id
            };

            _db.Bestellingen.Add(bestelling);

            try
            {
                await _db.SaveChangesAsync();
            }
            // ultra-zeldzame race: uniek index op OrderNummer faalt → probeer 1x opnieuw
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("IX_Bestellingen_OrderNummer") == true)
            {
                bestelling.OrderNummer = await _gen.GenerateAsync();
                await _db.SaveChangesAsync();
            }

            // mandje leeg en bedankje tonen met ordernummer
            HttpContext.Session.Remove("Cart");
            TempData["Bedankt"] = $"Bedankt voor je bestelling! 🍝  Order #{bestelling.OrderNummer}";

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
