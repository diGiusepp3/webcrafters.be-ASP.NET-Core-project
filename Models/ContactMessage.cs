using System;

namespace webcrafters.be_ASP.NET_Core_project.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }           // Primaire sleutel
        public string Name { get; set; }      // Naam van de afzender
        public string Email { get; set; }     // E-mailadres
        public string Message { get; set; }   // Bericht
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatisch gezet
    }
}