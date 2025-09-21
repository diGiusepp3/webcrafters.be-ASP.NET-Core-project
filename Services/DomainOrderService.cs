// File: Services/DomainOrderService.cs
using System.Net.Mail;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class DomainOrderService
    {
        private readonly IConfiguration _config;

        public DomainOrderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderMailAsync(string domain, string mode, string? authCode, string holderName, string holderEmail)
        {
            var devEmail = _config["DomainOrders:NotifyEmail"] ?? "info@webcrafters.be";

            var subject = mode == "register"
                ? $"🌐 Nieuwe domeinregistratie aanvraag: {domain}"
                : $"🔄 Nieuwe verhuisaanvraag: {domain}";

            var body = $@"
Beste team,

Er is een nieuwe domeinorder via de website:

Domein: {domain}
Type: {mode}
AuthCode: {authCode ?? "-"}
Klantnaam: {holderName}
Klant e-mail: {holderEmail}

Gelieve dit manueel te verwerken bij Vimex / TransIP.
";

            using var client = new SmtpClient("smtp.webcrafters.be")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential(
                    _config["Smtp:User"], 
                    _config["Smtp:Pass"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage("noreply@webcrafters.be", devEmail, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}