using Microsoft.EntityFrameworkCore;
using webcrafters.be_ASP.NET_Core_project.Models;
using webcrafters.be_ASP.NET_Core_project.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Database connectie
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

// ✅ SiteSettings configureren uit appsettings.json
builder.Services.Configure<SiteSettings>(
    builder.Configuration.GetSection("SiteSettings")
);

// ✅ TransIP services registreren
builder.Services.AddHttpClient<TransipAuthService>();
builder.Services.AddHttpClient<TransipDomainService>();

// ✅ Twilio SMS-services
builder.Services.AddSingleton<SmsService>();

// ✅ MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ✅ Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // voor bestanden in wwwroot
app.UseRouting();
app.UseAuthorization();

// ✅ routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();