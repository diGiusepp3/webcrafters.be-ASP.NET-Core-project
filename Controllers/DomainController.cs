using Microsoft.AspNetCore.Mvc;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class DomainController : Controller
    {
        [HttpPost]
        public IActionResult Check(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                TempData["Error"] = "Geef een domeinnaam in.";
                return RedirectToAction("Index", "Home", new { section = "domeinnaam" });
            }

            try
            {
                // Als DNS lookup lukt → domein bestaat al
                var host = System.Net.Dns.GetHostEntry(domain);

                TempData["Error"] = $"❌ Domeinnaam {domain} is al geregistreerd.";
                TempData["DomainTaken"] = domain; // zet domeinnaam door naar view
            }
            catch (System.Net.Sockets.SocketException)
            {
                // Geen DNS record → domein is beschikbaar
                TempData["Success"] = $"✅ Domeinnaam {domain} is beschikbaar!";
            }

            return RedirectToAction("Index", "Home", new { section = "domeinnaam" });
        }

        [HttpGet]
        public IActionResult Transfer(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                TempData["Error"] = "Geen domeinnaam opgegeven om te verhuizen.";
                return RedirectToAction("Index", "Home", new { section = "domeinnaam" });
            }

            ViewBag.Domain = domain;
            return View(); // je maakt straks Views/Domain/Transfer.cshtml
        }
    }
}