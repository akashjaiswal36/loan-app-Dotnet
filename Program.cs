using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Create the builder
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // ✅ Correct way to bind to all interfaces on port 5000
    Urls = new[] { "http://0.0.0.0:5000" }
});

// Add services
builder.Services.AddControllersWithViews();

// Build the app
var app = builder.Build();

// Configure the middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Loan}/{action=Index}/{id?}");

app.Run();
