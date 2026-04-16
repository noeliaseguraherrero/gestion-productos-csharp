using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos; // Asegúrate de que esto esté aquí
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// DbContext (Usando tu ApplicationDbContext de la carpeta Datos)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ??? AUTENTICACIÓN CON COOKIES
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Index";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

// ?? AQUÍ YA NO HAY NADA MÁS. 
// Las clases duplicadas han sido eliminadas para que el proyecto 
// use las que están en tu carpeta 'Modelos'.