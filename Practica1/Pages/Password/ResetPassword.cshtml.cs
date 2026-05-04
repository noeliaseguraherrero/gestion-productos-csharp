using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Services;

namespace Practica1.Pages.Password
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ResetPasswordModel(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                ErrorPassword = "Las contrasenas no coinciden.";
                return Page();
            }

            // 2. Buscar usuario con token válido
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

            // 4. Enviar correo de confirmacion
            await _emailService.EnviarEmailAsync(
                destinatario: usuario.Email!, asunto: "Contraseña actualizada - BecariosBD",
cuerpoHtml: $@"
<div style='font-family:Inter,Arial,sans-serif;background:#f1f5f9;padding:40px 20px;'>
    <div style='max-width:480px;margin:0 auto;'>

        <!-- CABECERA -->
        <div style='background:#1d4ed8;border-radius:16px 16px 0 0;
                    padding:28px 35px;text-align:center;'>
            <h1 style='color:#fff;font-size:1.1rem;font-weight:800;margin:0;
                       text-transform:uppercase;letter-spacing:2px;'>
                BecariosBD
            </h1>
        </div>

        <!-- CUERPO -->
        <div style='background:#fff;border-radius:0 0 16px 16px;
                    padding:40px 35px;border:1px solid #e2e8f0;border-top:none;'>

            <h2 style='color:#0f172a;font-size:1.15rem;font-weight:800;
                       margin:0 0 8px 0;letter-spacing:-0.5px;'>
                Contraseña actualizada
            </h2>
            <p style='color:#64748b;font-size:0.88rem;line-height:1.7;margin:0 0 25px 0;'>
                Tu contraseña ha sido modificada correctamente. Ya puedes acceder al sistema.
            </p>

            <!-- CONFIRMACION -->
            <div style='background:#f0fdf4;border:1px solid #bbf7d0;border-radius:10px;
                        padding:14px 18px;margin-bottom:25px;text-align:center;'>
                <span style='color:#166534;font-weight:700;font-size:0.85rem;'>
                    Cambio realizado con exito
                </span>
            </div>

            <!-- AVISO SEGURIDAD -->
            <div style='background:#fff1f2;border:1px solid #fecdd3;border-radius:10px;
                        padding:14px 18px;margin-bottom:28px;'>
                <p style='color:#be123c;font-weight:700;font-size:0.82rem;margin:0 0 4px 0;'>
                    No reconoces este cambio?
                </p>
                <p style='color:#64748b;font-size:0.78rem;margin:0;line-height:1.6;'>
                    Contacta con el administrador del sistema de inmediato.
                </p>
            </div>

            <!-- BOTON -->
            <div style='text-align:center;margin-bottom:25px;'>
                <a href='{Request.Scheme}://{Request.Host}'
                   style='display:inline-block;padding:14px 36px;
                          background:#1d4ed8;color:#fff;border-radius:10px;
                          text-decoration:none;font-weight:700;font-size:0.85rem;
                          text-transform:uppercase;letter-spacing:1.5px;'>
                    Ir al Inicio de Sesión
                </a>
            </div>

            <!-- PIE -->
            <p style='color:#94a3b8;font-size:0.78rem;text-align:center;
                      margin:0;line-height:1.7;border-top:1px solid #f1f5f9;padding-top:20px;'>
                Este es un correo automático, no respondas a este mensaje.
            </p>
        </div>
    </div>
</div>"
            );

            Exito = true;
            return Page();
        }
    }
}