using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CiberAgentes.Data;
using CiberAgentes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Añadir la cadena de conexión para MySQL - se modificará más adelante
// con la información real de Kamatera
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encuentra en la configuración.");

// Configuración de DbContext con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

// Configurar Identity para usuarios
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = true; // Requerimos confirmación de email
    options.Password.RequiredLength = 8; // Mínimo 8 caracteres
    options.Password.RequireNonAlphanumeric = true; // Caracteres especiales
    options.Password.RequireUppercase = true; // Mayúsculas
    options.Password.RequireLowercase = true; // Minúsculas
    options.Password.RequireDigit = true; // Números
})
.AddRoles<IdentityRole>() // Agregamos soporte para roles
.AddEntityFrameworkStores<ApplicationDbContext>();

// Política global que requiere autenticación
var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Añadir servicios MVC con la política global de autenticación
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
