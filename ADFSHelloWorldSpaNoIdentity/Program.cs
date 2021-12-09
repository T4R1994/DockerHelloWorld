using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(sharedOptions =>
    {
        sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
    })
    .AddWsFederation(options =>
     {
         //options.Wtrealm = "https://localhost:44490/";
         //options.MetadataAddress = "https://adfs.astor.com.pl/FederationMetadata/2007-06/FederationMetadata.xml";
         options.Wtrealm = "https://localhost:44418/";
         options.MetadataAddress = "https://adfs.astor.com.pl/FederationMetadata/2007-06/FederationMetadata.xml";
         options.Events.OnTicketReceived += async (ticket) => {
             var ss = "test";
         };
         options.RequireHttpsMetadata = false;
     })
     .AddCookie();

builder.Services.AddCors(options =>
{
    //options.AddPolicy(name: "_myAllowSpecificOrigins",
    //                  builder =>
    //                  {
    //                      builder.WithOrigins("https://localhost:44490",
    //                                          "https://adfs.astor.com.pl");
    //                  });
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
