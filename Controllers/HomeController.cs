// FILE: Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using webcrafters.be_ASP.NET_Core_project.Models;

namespace webcrafters.be_ASP.NET_Core_project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SiteSettings _settings;
    private readonly AppDbContext _context;   // ✅ juiste naam

    public HomeController(
        ILogger<HomeController> logger, 
        IOptions<SiteSettings> settings,
        AppDbContext context)   // ✅ juiste naam
    {
        _logger = logger;
        _settings = settings.Value;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Webcrafters — Home";

        var blogs = await _context.Blogs
            .OrderByDescending(b => b.BlogToegevoegd)
            .ToListAsync();

        _settings.Blogs = blogs;

        return View(_settings);
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