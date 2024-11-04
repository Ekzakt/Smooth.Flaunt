using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Smooth.Flaunt.Server.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteOptions>(routeOptions =>
{
    routeOptions.LowercaseUrls = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddBff();

IdentityModelEventSource.ShowPII = builder.Environment.IsDevelopment();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>()
    .SetApplicationName("SmoothSensation.SharedCookie");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = ".AspNet.SharedCookie";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.Domain = builder.Configuration["IdentityServer:CookieDomain"];
        options.Cookie.HttpOnly = true;
        options.Cookie.Path = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Increased from 1 minute
        options.SlidingExpiration = true;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.ClientId = builder.Configuration["IdentityServer:ClientId"];
        options.ClientSecret = builder.Configuration["IdentityServer:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.Scope.Add("flauntapi.read");

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = false;
        options.DisableTelemetry = true;
    });



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();


app.MapRazorPages();

app.MapControllers()
    .RequireAuthorization()
    .AsBffApiEndpoint();

app.MapFallbackToFile("index.html");

app.Run();
