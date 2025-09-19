using Microsoft.EntityFrameworkCore;
using webcrafters.be_ASP.NET_Core_project.Models;
using webcrafters.be_ASP.NET_Core_project.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// ✅ Laad .env (alleen aanwezig bij local dev, niet in productie)
Env.Load();

// ✅ Config sources (JSON + Environment Variables)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// ✅ Database connectie uit ENV
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)), // pas versie aan indien nodig
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

Console.WriteLine("ConnectionString:");
Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection") ?? "NULL");

// ✅ SiteSettings uit appsettings.json
builder.Services.Configure<SiteSettings>(
    builder.Configuration.GetSection("SiteSettings")
);

// ✅ TransIP services
builder.Services.AddHttpClient<TransipAuthService>();
builder.Services.AddHttpClient<TransipDomainService>();

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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
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

app.Run();
