using Microsoft.AspNetCore.Mvc;
using webcrafters.be_ASP.NET_Core_project.Models;
using webcrafters.be_ASP.NET_Core_project.Services;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _db;
        private readonly SmsService _smsService;

        public ContactController(AppDbContext db, SmsService smsService)
        {
            _db = db;
            _smsService = smsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Name, string Email, string Message)
        {
            var contactMessage = new ContactMessage
            {
                Name = Name,
                Email = Email,
                Message = Message,
                CreatedAt = DateTime.UtcNow
            };

            _db.ContactMessages.Add(contactMessage);
            await _db.SaveChangesAsync();

            // ✅ SMS sturen via Twilio
            var smsText = $"📩 Nieuw bericht via Webcrafters.be van {Name} ({Email}): {Message}";
            _smsService.SendSms("+32456207354", smsText);

            TempData["Success"] = "Uw bericht is verzonden ✅";
            return RedirectToAction("Index");
        }

    }
}
