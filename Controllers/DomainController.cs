// FILE: Controllers/DomainController.cs
using Microsoft.AspNetCore.Mvc;
using webcrafters.be_ASP.NET_Core_project.Services;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    [Route("[controller]")]
    public class DomainController : Controller
    {
        private readonly TransipDomainService _domainService;

        public DomainController(TransipDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpPost("Check")]
        public async Task<IActionResult> Check(string domain)
        {
            var (success, message) = await _domainService.CheckAvailabilityAsync(domain);
            return Json(new { success, message });
        }
    }
}