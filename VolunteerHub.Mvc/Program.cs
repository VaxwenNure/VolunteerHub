var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddSingleton<VolunteerHub.Mvc.Services.IJsonStore, VolunteerHub.Mvc.Services.JsonStore>();
builder.Services.AddScoped<VolunteerHub.Mvc.Services.ReportService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Volunteers}/{action=Index}/{id?}");

app.Run();
