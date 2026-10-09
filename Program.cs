using HeroesWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("HeroesDb")
    ?? throw new InvalidOperationException("No se encontró la conexión HeroesDb.");

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Heroes");
    options.Conventions.AuthorizeFolder("/SuperPoderes");
});

// El proyecto continúa usando Razor Pages y, además, habilita controladores con
// vistas para el laboratorio de seguimiento de cambios de EF Core.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HeroesContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Las páginas de Identity ya fueron generadas en el proyecto. Como esta práctica
// no confirma correos, se usa un remitente local que satisface esa dependencia.
builder.Services.AddSingleton<IEmailSender, Microsoft.AspNetCore.Identity.UI.Services.NoOpEmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "mvc",
    pattern: "{controller}/{action}/{id?}");
app.MapRazorPages().WithStaticAssets();
app.Run();
