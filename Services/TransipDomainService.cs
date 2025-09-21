// File: Services/TransipDomainService.cs
using System.Net;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class TransipDomainService
    {
        private readonly HttpClient _httpClient;
        private readonly TransipAuthService _auth;

        public TransipDomainService(TransipAuthService auth)
        {
            _auth = auth;

            // Forceer IPv4 connecties
            var handler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host);
                    var ipv4 = addresses.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync(new IPEndPoint(ipv4, context.DnsEndPoint.Port), cancellationToken);
                    return new NetworkStream(socket, ownsSocket: true);
                }
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.transip.nl/v6/")
            };
        }

        private async Task EnsureAuthAsync()
        {
            var token = await _auth.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private string NormalizeDomain(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            input = input.Trim().ToLowerInvariant();
            input = Regex.Replace(input, @"^https?://", "");
            if (input.StartsWith("www.")) input = input[4..];
            var slashIndex = input.IndexOf('/');
            if (slashIndex >= 0) input = input[..slashIndex];
            return input;
        }

        public record DomainCheckResult(
            bool IsFree,
            string Status,
            decimal? PriceExVat = null,
            string? Currency = null,
            string? RawResponse = null
        )
        {
            public decimal PriceInclVat => PriceExVat.HasValue ? PriceExVat.Value * 1.21m : 0;
            public decimal FinalPrice => PriceInclVat * 1.40m; // marge +40%
        }


        /// <summary>
        /// Check domeinnaam beschikbaarheid via TransIP
        /// </summary>
        public async Task<DomainCheckResult> CheckAvailabilityAsync(string domain)
        {
            await EnsureAuthAsync();

            domain = NormalizeDomain(domain);
            var resp = await _httpClient.GetAsync($"domain-availability/{domain}");
            var raw = await resp.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            string status = "unknown";
            if (root.TryGetProperty("status", out var statusNode))
                status = statusNode.GetString() ?? "unknown";
            else if (root.TryGetProperty("error", out var errorNode))
                status = "error: " + errorNode.GetString();

            bool isFree = string.Equals(status, "free", StringComparison.OrdinalIgnoreCase);

            decimal? price = null;
            string? currency = null;

            if (root.TryGetProperty("price", out var priceNode))
            {
                if (priceNode.TryGetProperty("buyPrice", out var buyPriceNode) &&
                    buyPriceNode.TryGetDecimal(out var dec))
                {
                    price = dec;
                }
                if (priceNode.TryGetProperty("currency", out var curNode))
                {
                    currency = curNode.GetString();
                }
            }

            return new DomainCheckResult(isFree, status, price, currency, raw);
        }
    }
}
