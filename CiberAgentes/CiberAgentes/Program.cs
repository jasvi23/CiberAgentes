using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CiberAgentes.Data;
using CiberAgentes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;


var builder = WebApplication.CreateBuilder(args);

// A�adir la cadena de conexi�n para MySQL - se modificar� m�s adelante
// con la informaci�n real de Kamatera
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("La cadena de conexi�n 'DefaultConnection' no se encuentra en la configuraci�n.");

// Configuraci�n de DbContext con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

// Configurar Identity para usuarios
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = true; // Requerimos confirmaci�n de email
    options.Password.RequiredLength = 8; // M�nimo 8 caracteres
    options.Password.RequireNonAlphanumeric = true; // Caracteres especiales
    options.Password.RequireUppercase = true; // May�sculas
    options.Password.RequireLowercase = true; // Min�sculas
    options.Password.RequireDigit = true; // N�meros
})
.AddRoles<IdentityRole>() // Agregamos soporte para roles
.AddEntityFrameworkStores<ApplicationDbContext>();

// Pol�tica global que requiere autenticaci�n
var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// A�adir servicios MVC con la pol�tica global de autenticaci�n
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy));
});

// Configurar servicios adicionales de Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure el pipeline de solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Definir rutas para los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Definir rutas para Razor Pages (usado por Identity)
app.MapRazorPages();

app.Run();
