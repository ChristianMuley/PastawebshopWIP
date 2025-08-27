using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MVCappTemplate.Models;

namespace MVCappTemplate.Controllers
{
    public class BestellingController : Controller
    {
        //GET: Winkelmandje
        [HttpGet]
        public IActionResult Index()
        {
            return View(new BestelItem());
        }

        // POST: Winkelmandje
        [HttpPost]
        public IActionResult Toevoegen(BestelItem item)
        {
            // Leest cart/winkelmandje als JSON van de session
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data)
                ? new List<BestelItem>()
                : JsonSerializer.Deserialize<List<BestelItem>>(data)!;

            // Voegt selectie toe
            cart.Add(item);

            // Terug in session opslaan
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            
            return RedirectToAction("Mandje");
        }

        // GET: Winkelmandje
        [HttpGet]
        public IActionResult Mandje()
        {
            var data = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(data)
                ? new List<BestelItem>()
                : JsonSerializer.Deserialize<List<BestelItem>>(data)!;

            return View(cart);
        }

        // POST: Bestellingdoorgeven
        [HttpPost]
        public IActionResult BestellingDoorvoeren()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Bedankt"] = "Bedankt voor je bestelling! 🍝";
            return RedirectToAction("Mandje");
        }

        // GET: Nieuwsbrief
        [HttpGet]
        public IActionResult Nieuwsbrief() => View();

        // POST: Nieuwsbriefbevestiging
        [HttpPost]
        public IActionResult Nieuwsbrief(NieuwsbriefModel model)
        {
            if (!ModelState.IsValid) return View(model);
            return View("NieuwsbriefBevestiging", model);
        }
    }
}
