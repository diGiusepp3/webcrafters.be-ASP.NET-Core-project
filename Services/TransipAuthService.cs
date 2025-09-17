// FILE: Services/TransipAuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Json;
using System.Text.Json;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class TransipAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private string? _cachedToken;
        private DateTime _expiresAt = DateTime.MinValue;

        public TransipAuthService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.transip.nl/v6/");
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // Reuse token if still valid
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
            {
                return _cachedToken;
            }

            var login = _config["Transip:Login"];
            var privateKeyPath = _config["Transip:PrivateKeyPath"];
            var fullPath = Path.GetFullPath(privateKeyPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"❌ Key file not found at {fullPath}");
            }

            Console.WriteLine($"✅ Key file gevonden: {fullPath}");

            var privateKey = await File.ReadAllTextAsync(fullPath);
            Console.WriteLine($"✅ Key file ingelezen, lengte: {privateKey.Length} tekens");

            var readOnly = bool.Parse(_config["Transip:ReadOnly"] ?? "false");

            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            // Maak JWT
            var handler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = login,
                Claims = new Dictionary<string, object>
                {
                    { "sub", login },
                    { "login", login },
                    { "read_only", readOnly }
                },
                Expires = DateTime.UtcNow.AddMinutes(5), // JWT zelf is kort geldig
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            };

            var jwt = handler.CreateJwtSecurityToken(tokenDescriptor);
            var jwtString = handler.WriteToken(jwt);
            
            Console.WriteLine("JWT payload: " + handler.WriteToken(jwt));


            // Nonce + login in body
            var nonce = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, "auth")
            {
                Content = JsonContent.Create(new { login, nonce })
            };
            request.Headers.Add("Signature", jwtString);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"TransIP auth failed ({response.StatusCode}): {body}");
            }

            var json = JsonDocument.Parse(body);
            var token = json.RootElement.GetProperty("token").GetString();
            var expiration = json.RootElement.TryGetProperty("expirationTime", out var expProp)
                ? expProp.GetInt64()
                : 3600; // fallback: 1h

            _cachedToken = token!;
            _expiresAt = DateTime.UtcNow.AddSeconds(expiration - 60); // 1 min speling

            return _cachedToken;
        }
    }
}
