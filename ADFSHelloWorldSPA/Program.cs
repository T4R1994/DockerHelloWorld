using ADFSHelloWorldSPA.Data;
using ADFSHelloWorldSPA.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    //.AddIdentityServerJwt()
    .AddWsFederation(options =>
    {
        // MetadataAddress represents the Active Directory instance used to authenticate users.
        options.MetadataAddress = "https://adfs.astor.com.pl/FederationMetadata/2007-06/FederationMetadata.xml";
        // Wtrealm is the app's identifier in the Active Directory instance.
        // For ADFS, use the relying party's identifier, its WS-Federation Passive protocol URL:
        options.Wtrealm = "https://localhost:44342/";
        // For AAD, use the Application ID URI from the app registration's Overview blade:
        //options.Wtrealm = "api://bbd35166-7c13-49f3-8041-9551f2847b69";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html"); ;

app.Run();
