using E_Commerce.Core.Entities;
using E_Commerce.Infrastructure;
using E_Commerce.Infrastructure.Persistence.Data;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using E_Commerce.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationbDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationbDbContext>();

builder.AddInfrastructureRegistraion();
builder.AddServiceRegistraion();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLogging(); // Ensures logging services are available

// Configure logging to the console
builder.Logging.AddConsole();

// Register HttpClient 
builder.Services.AddHttpClient();

Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();