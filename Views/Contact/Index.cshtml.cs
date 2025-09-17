// FILE: Views/Pages/Contact/Contact.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webcrafters.be_ASP.NET_Core_project.Pages.Contact
{
    public class ContactModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Hier kan je e-mail versturen of DB-entry maken
            TempData["Success"] = $"Bericht van {Name} is verzonden!";
            return RedirectToPage("/Contact/Contact");
        }
    }
}