using Microsoft.EntityFrameworkCore;
using VolunteerHub.Data.Data;
using VolunteerHub.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<VolunteerHubContext>(options =>
    options.UseSqlite("Data Source=volunteerhub.db"));

builder.Services.AddScoped<ReportService>();

var app = builder.Build();

// Auto-create DB and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VolunteerHubContext>();
    db.Database.EnsureCreated();
    DbInitializer.Seed(db);
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
