// File: Services/TransipAuthService.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class TransipAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private string? _cachedToken;
        private DateTime _expiresAt = DateTime.MinValue;

        public TransipAuthService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient { BaseAddress = new Uri("https://api.transip.nl/v6/") };
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // ✅ Cache check
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
            {
                Console.WriteLine("🔑 Cached token hergebruikt (geldig tot " + _expiresAt + ")");
                return _cachedToken;
            }

            var login = _config["Transip:Login"];
            var privateKeyPath = _config["Transip:PrivateKeyPath"];
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(privateKeyPath))
                throw new Exception("❌ TransIP config ontbreekt (Login of PrivateKeyPath)");

            var fullPath = Path.GetFullPath(privateKeyPath);
            if (!File.Exists(fullPath))
                throw new Exception($"❌ Key file niet gevonden op: {fullPath}");

            var privateKey = await File.ReadAllTextAsync(fullPath);

            // ✅ Nonce 16 tekens
            var nonce = GenerateNonce();
            var bodyObj = new { login, nonce };
            var bodyJson = JsonSerializer.Serialize(bodyObj);

            Console.WriteLine("📦 Request body: " + bodyJson);

            // ✅ Signeren
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            var data = Encoding.UTF8.GetBytes(bodyJson);
            var signatureBytes = rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            var signatureBase64 = Convert.ToBase64String(signatureBytes);

            Console.WriteLine("🔏 Signature (eerste 60 chars): " + signatureBase64.Substring(0, Math.Min(60, signatureBase64.Length)));

            // ✅ Request
            var request = new HttpRequestMessage(HttpMethod.Post, "auth")
            {
                Content = new StringContent(bodyJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Signature", signatureBase64);

            var response = await _httpClient.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            Console.WriteLine("🌐 TransIP response: " + raw);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"❌ Auth failed ({response.StatusCode}): {raw}");

            // ✅ JSON parsing
            using var doc = JsonDocument.Parse(raw);

            if (!doc.RootElement.TryGetProperty("token", out var tokenProp))
                throw new Exception("❌ Response bevat geen 'token': " + raw);

            _cachedToken = tokenProp.GetString();

            if (doc.RootElement.TryGetProperty("expirationTime", out var expProp) &&
                expProp.TryGetInt64(out var expSecs))
            {
                _expiresAt = DateTime.UtcNow.AddSeconds(expSecs - 60);
            }
            else
            {
                _expiresAt = DateTime.UtcNow.AddMinutes(10); // fallback
            }

            Console.WriteLine("✅ Token ontvangen (eerste 40 chars): " +
                _cachedToken?.Substring(0, Math.Min(40, _cachedToken.Length)));

            return _cachedToken!;
        }

        private string GenerateNonce()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[16];
            rng.GetBytes(bytes);
            return Convert.ToHexString(bytes).ToLower().Substring(0, 16); // 16 tekens
        }
    }
}
