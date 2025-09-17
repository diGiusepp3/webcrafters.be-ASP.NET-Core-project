// FILE: Services/TransipDomainService.cs
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class TransipDomainService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly TransipAuthService _authService;


        public TransipDomainService(IConfiguration config, HttpClient httpClient, TransipAuthService authService)
        {
            _config = config;
            _httpClient = httpClient;
            _authService = authService;
            _httpClient.BaseAddress = new Uri("https://api.transip.nl/v6/");
        }

        public async Task<(bool success, string message)> CheckAvailabilityAsync(string domain)
        {
            var token = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            domain = NormalizeDomain(domain);
            var response = await _httpClient.GetAsync($"domains/{domain}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return (false, $"Fout {response.StatusCode}: {body}");

            return (true, body);
        }

        private string NormalizeDomain(string input)
        { 
            if (string.IsNullOrWhiteSpace(input)) return input;

            input = input.Trim().ToLowerInvariant();

            // Verwijder protocol
            input = Regex.Replace(input, @"^https?://", "");

            // Verwijder www. 
            if (input.StartsWith("www."))
                input = input.Substring(4);

            // Verwijder trailing slash en pad
            var slashIndex = input.IndexOf('/');
            if (slashIndex >= 0)
                input = input.Substring(0, slashIndex);

            return input;
        }
    }
}
