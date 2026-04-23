using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;

namespace Practica1.Pages.Password
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResetPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;

        [BindProperty]
        public string NuevaPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmarPassword { get; set; } = string.Empty;

        public bool TokenInvalido { get; set; } = false;
        public bool Exito { get; set; } = false;
        public string? ErrorPassword { get; set; }

        public async Task OnGetAsync()
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.ResetToken == Token
                                       && u.ResetTokenExpiry > DateTime.Now);
            if (usuario == null)
                TokenInvalido = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Validar que coinciden
            if (NuevaPassword != ConfirmarPassword)
            {
                ErrorPassword = "Las contraseñas no coinciden.";
                return Page();
            }

            // 2. Buscar usuario con token válido y no caducado
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.ResetToken == Token
                                       && u.ResetTokenExpiry > DateTime.Now);
            if (usuario == null)
            {
                TokenInvalido = true;
                return Page();
            }

            // 3. Guardar nueva contraseña y limpiar token
            usuario.Contraseña = NuevaPassword;
            usuario.ResetToken = null;
            usuario.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            Exito = true;
            return Page();
        }
    }
}
