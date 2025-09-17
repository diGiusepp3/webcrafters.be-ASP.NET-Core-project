// FILE: Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using webcrafters.be_ASP.NET_Core_project.Models;

namespace webcrafters.be_ASP.NET_Core_project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SiteSettings _settings;

    public HomeController(ILogger<HomeController> logger, IOptions<SiteSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Webcrafters — Home";
        return View(_settings); // geef settings door aan Index.cshtml
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}