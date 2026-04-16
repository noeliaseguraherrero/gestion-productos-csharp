using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practica1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string Usuario { get; set; }
            public string Contrasena { get; set; }
        }

        // Se ejecuta al cargar la página (GET)
        public void OnGet()
        {
            // Dejado vacío para que SIEMPRE muestre el formulario de login 
            // aunque el usuario ya tenga una cookie antigua.
        }

        // Se ejecuta al enviar el formulario (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Buscamos al usuario por nombre y contraseña
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Nombre == Input.Usuario && u.Contraseña == Input.Contrasena);

            if (usuario != null)
            {
                // Verificamos si la cuenta está activa (campo bit en la BD)
                if (!usuario.Activo)
                {
                    ErrorMessage = "Tu cuenta ha sido desactivada.";
                    return Page();
                }

                // Creamos los Claims (la identidad del usuario)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    // Importante para el sistema de roles Admin/Usuario
                    new Claim(ClaimTypes.Role, usuario.Rol ?? "Usuario")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // CONFIGURACIÓN DE SEGURIDAD DE LA COOKIE
                var authProperties = new AuthenticationProperties
                {
                    // IMPORTANTE: IsPersistent = false hace que la sesión 
                    // se borre al cerrar el navegador. No se guarda en disco.
                    IsPersistent = false,
                    AllowRefresh = false
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                return RedirectToPage("/Productos/Inicio");
            }

            ErrorMessage = "Usuario o contraseña incorrectos.";
            return Page();
        }

        // MÉTODO PARA CERRAR SESIÓN
        // Se activa al llamar a: asp-page="/Index" asp-page-handler="Logout"
        public async Task<IActionResult> OnGetLogoutAsync()
        {
            // Borra la cookie de autenticación del navegador
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirige a la misma página (Login) para limpiar la vista
            return RedirectToPage("/Index");
        }
    }
}