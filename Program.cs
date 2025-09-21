// File: Program.cs
using Microsoft.EntityFrameworkCore;
using webcrafters.be_ASP.NET_Core_project.Models;
using webcrafters.be_ASP.NET_Core_project.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("🚀 Startup: Program.cs gestart");

// ✅ Laad .env (alleen aanwezig bij local dev, niet in productie)
try
{
    Env.Load();
    Console.WriteLine("📂 .env geladen (indien aanwezig).");
}
catch (Exception ex)
{
    Console.WriteLine("⚠️ Kon .env niet laden: " + ex.Message);
}

// ✅ Config sources (JSON + Environment Variables)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// ✅ Database connectie uit ENV
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conn))
{
    Console.WriteLine("❌ Geen ConnectionString gevonden! Controleer appsettings.json of .env");
}
else
{
    Console.WriteLine("🔗 ConnectionString gevonden:");
    Console.WriteLine(conn);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        conn,
        new MySqlServerVersion(new Version(8, 0, 36)), // pas versie aan indien nodig
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// ✅ SiteSettings uit appsettings.json
var siteSettings = builder.Configuration.GetSection("SiteSettings");
if (!siteSettings.Exists())
{
    Console.WriteLine("⚠️ Geen SiteSettings gevonden in appsettings.json");
}
builder.Services.Configure<SiteSettings>(siteSettings);

// ✅ TransIP services
var transipLogin = builder.Configuration["Transip:Login"];
var transipKey = builder.Configuration["Transip:PrivateKeyPath"];
Console.WriteLine($"🔑 TransIP settings: Login={transipLogin}, Key={transipKey}");
builder.Services.AddSingleton<TransipAuthService>();
builder.Services.AddSingleton<TransipDomainService>();

// ✅ Twilio SMS-service
builder.Services.AddSingleton<SmsService>();

// ✅ MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ✅ Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionService>();

var app = builder.Build();

// ✅ Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    Console.WriteLine("🌍 Running in PRODUCTION mode");
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    Console.WriteLine("💻 Running in DEVELOPMENT mode");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

// ✅ routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

Console.WriteLine("✅ App klaar voor gebruik. Listening...");
app.Run();
