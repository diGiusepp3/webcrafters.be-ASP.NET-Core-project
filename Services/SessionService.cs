// FILE: Services/SessionService.cs
using Microsoft.AspNetCore.Http;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            return string.IsNullOrEmpty(userId) ? null : int.Parse(userId);
        }

        public string? GetUsername()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Username");
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(GetUsername());
        }
    }
}