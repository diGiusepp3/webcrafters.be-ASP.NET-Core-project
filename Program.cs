using webcrafters.be_ASP.NET_Core_project.Models;
using webcrafters.be_ASP.NET_Core_project.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<SiteSettings>(
    builder.Configuration.GetSection("SiteSettings")
);
builder.Services.AddHttpClient<TransipAuthService>();
builder.Services.AddHttpClient<TransipDomainService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // ✅ voor wwwroot
app.UseRouting();
app.UseAuthorization();

// ✅ routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();