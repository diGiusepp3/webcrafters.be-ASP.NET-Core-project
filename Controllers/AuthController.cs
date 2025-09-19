// FILE: Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webcrafters.be_ASP.NET_Core_project.Models;
using System.Security.Cryptography;
using System.Text;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Register
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // check of email of username al bestaat
            if (await _context.Users.AnyAsync(u => u.Email == model.Email || u.Username == model.Username))
            {
                ViewBag.Error = "Email of gebruikersnaam bestaat al";
                return View(model);
            }

            model.Password = ComputeSha256Hash(model.Password);
            model.Registered = DateTime.UtcNow;
            model.Confirmed = true;

            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserId", model.UserId.ToString());
            HttpContext.Session.SetString("Username", model.Username);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hash = ComputeSha256Hash(password);

            // Debug logging
            Console.WriteLine($"[LOGIN DEBUG] Ingegeven: {email}, Hash: {hash}");

            var user = await _context.Users.FirstOrDefaultAsync(
                u => (u.Email == email || u.Username == email)
            );

            if (user == null)
            {
                ViewBag.Error = $"Geen gebruiker gevonden voor {email}";
                return View();
            }

            Console.WriteLine($"[LOGIN DEBUG] DB User: {user.Username}, Email: {user.Email}, Hash in DB: {user.Password}");

            if (user.Password != hash)
            {
                ViewBag.Error = "Wachtwoord klopt niet";
                return View();
            }

            user.LastOnline = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }
    }
}
