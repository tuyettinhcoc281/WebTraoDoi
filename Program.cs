using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExchangeWebsite.Controllers;
using ExchangeWebsite.Models; // Make sure this is at the top

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ExchangeWebsiteContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ExchangeWebsiteContextConnection' not found.");

// Register the application's DbContext
builder.Services.AddDbContext<ExchangeWebsiteContext>(options =>
    options.UseSqlServer(connectionString));

// Register Identity with the custom User class
builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // You can add more Identity options here if needed
    })
    .AddEntityFrameworkStores<ExchangeWebsiteContext>();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHostedService<PostCleanupService>();
builder.Services.AddScoped<VnPayService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
