using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Proyecto_Tienda;
using Proyecto_Tienda.Services;

var builder = WebApplication.CreateBuilder(args);

// Add this
var authenticatedUsers = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opciones => {
    opciones.Filters.Add(new AuthorizeFilter(authenticatedUsers));
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(opciones =>
{
    opciones.DataAnnotationLocalizerProvider = (_, factoria) =>
    factoria.Create(typeof(SharedResource));
});

// Establish applicationDbContext as a service
builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
opciones.UseSqlServer("name=DefaultConnection"));

// Enable authentication services (login/register) normal and with Microsoft Accounts
builder.Services.AddAuthentication()
    .AddMicrosoftAccount(opciones => {
    opciones.ClientId = builder.Configuration["MicrosoftClientId"];
    opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
});


// .AddGoogle(opciones => {
// opciones.ClientId = builder.Configuration["GoogleClientId"];
// opciones.ClientSecret = builder.Configuration["GoogleClientSecret"];


builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones => {
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/users/login";
        opciones.AccessDeniedPath = "/users/login";
    });

// To use multi languages
builder.Services.AddLocalization(opciones =>
{
    opciones.ResourcesPath = "Resources";
});

var app = builder.Build();



// We establish default language and the others supported
app.UseRequestLocalization(opciones =>
{
    opciones.DefaultRequestCulture = new RequestCulture("en");
    opciones.SupportedUICultures = Constants.SupportedUICultures
        .Select(cultura => new CultureInfo(cultura.Value)).ToList();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Get authenticated user data
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
