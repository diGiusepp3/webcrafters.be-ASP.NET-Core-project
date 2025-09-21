// File: Controllers/DomainController.cs
using Microsoft.AspNetCore.Mvc;
using webcrafters.be_ASP.NET_Core_project.Services;
using webcrafters.be_ASP.NET_Core_project.Models;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class DomainController : Controller
    {
        private readonly TransipDomainService _transip;

        public DomainController(TransipDomainService transip)
        {
            _transip = transip;
        }

        /// <summary>
        /// Domeinnaam checken via TransIP API
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                TempData["Error"] = "❌ Geen domeinnaam opgegeven.";
                return RedirectToAction("Index", "Home", new { fragment = "domeinnaam" });
            }

            try
            {
                Console.WriteLine($"🔎 CheckAvailability gestart voor {domain}");
                var result = await _transip.CheckAvailabilityAsync(domain);

                if (result.PriceExVat == null || result.PriceExVat <= 0)
                {
                    TempData["Error"] = $"❌ Geen prijs gevonden voor {domain}.";
                    return RedirectToAction("Index", "Home", new { fragment = "domeinnaam" });
                }

                if (result.IsFree)
                {
                    TempData["Success"] =
                        $"✅ {domain} is beschikbaar voor {result.FinalPrice:0.00} {result.Currency}/jaar (incl. btw & marge).";
                }
                else
                {
                    TempData["Error"] =
                        $"❌ {domain} is niet beschikbaar. Huidige prijs voor verhuizing: {result.FinalPrice:0.00} {result.Currency}/jaar (incl. btw & marge).";
                    TempData["DomainTaken"] = domain;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("💥 Exception in Check: " + ex);
                TempData["Error"] = $"Exception: {ex.Message}";
            }

            return RedirectToAction("Index", "Home", new { fragment = "domeinnaam" });
        }


        /// <summary>
        /// Verhuisformulier GET
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Transfer(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                TempData["Error"] = "❌ Geen domeinnaam opgegeven om te verhuizen.";
                return RedirectToAction("Index", "Home", new { fragment = "domeinnaam" });
            }

            // Vraag prijs op via TransIP API
            var check = await _transip.CheckAvailabilityAsync(domain);

            if (check.PriceExVat == null || check.PriceExVat <= 0)
            {
                TempData["Error"] = $"❌ Geen prijs gevonden voor {domain}.";
                return RedirectToAction("Index", "Home", new { fragment = "domeinnaam" });
            }

            var vm = new DomainTransferViewModel
            {
                Domain = domain,
                PriceExVat = check.PriceExVat.Value,
                Currency = check.Currency ?? "EUR"
            };

            return View(vm);
        }


        /// <summary>
        /// Verhuisformulier POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(DomainTransferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Vul alle velden in.";
                return View(model);
            }

            // 👉 Hier zou je later DB opslaan of een payment redirect doen
            TempData["Success"] = $"✅ Verhuisaanvraag voor {model.Domain} ontvangen. Wij nemen contact met je op.";

            return RedirectToAction("Index", "Home", new { fragment = "domeinnaam" });
        }
    }
}
