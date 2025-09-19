// FILE: Controllers/DemoController.cs
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class DemoController : Controller
    {
        [HttpPost]
        public IActionResult PlanAppointment(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                ViewBag.PlannedAppointment = "Geen invoer ontvangen.";
                return View("DemoResult");
            }

            // Eenvoudige "AI": zoek naar datumwoorden
            DateTime appointment = DateTime.Now;

            if (input.Contains("morgen", StringComparison.OrdinalIgnoreCase))
                appointment = DateTime.Now.AddDays(1);

            // Regex voor tijd (bijv. "14u", "14:30")
            var timeMatch = Regex.Match(input, @"(\d{1,2})([:u](\d{2}))?");
            if (timeMatch.Success)
            {
                int hour = int.Parse(timeMatch.Groups[1].Value);
                int minute = timeMatch.Groups[3].Success ? int.Parse(timeMatch.Groups[3].Value) : 0;
                appointment = new DateTime(appointment.Year, appointment.Month, appointment.Day, hour, minute, 0);
            }

            ViewBag.PlannedAppointment = appointment.ToString("dddd dd MMMM yyyy HH:mm");

            return View("DemoResult");
        }
    }
}