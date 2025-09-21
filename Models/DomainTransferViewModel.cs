// File: Models/DomainTransferViewModel.cs
namespace webcrafters.be_ASP.NET_Core_project.Models
{
    public class DomainTransferViewModel
    {
        public string Domain { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
        public string HolderEmail { get; set; } = string.Empty;
        public decimal PriceExVat { get; set; }
        public string Currency { get; set; } = "EUR";

        // Berekeningen
        public decimal PriceInclVat => PriceExVat * 1.21m;
        public decimal FinalPrice => PriceInclVat * 1.40m;
    }
}