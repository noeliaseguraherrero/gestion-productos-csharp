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
                destinatario: usuario.Email!,
                asunto: "Contrasena actualizada - BecariosBD",
                cuerpoHtml: $@"
                <div style='font-family: Inter, Arial, sans-serif; background-color: #f9f9f9;
                            padding: 40px 20px; min-height: 100vh;'>
                    <div style='max-width: 520px; margin: auto; background: #ffffff;
                                border-radius: 30px; padding: 50px;
                                box-shadow: 0 20px 60px rgba(0,0,0,0.08);
                                border: 1px solid #f0f0f0;'>

                        <h1 style='text-align: center; font-weight: 800; font-size: 1.8rem;
                                   color: #1a1a1a; text-transform: uppercase;
                                   letter-spacing: -1px; margin: 0 0 8px 0;'>
                            Contrasena Actualizada
                        </h1>
                        <p style='text-align: center; color: #999; font-size: 0.78rem;
                                  font-weight: 700; letter-spacing: 2px;
                                  text-transform: uppercase; margin: 0 0 40px 0;'>
                            BecariosBD · Gestion Administrativa
                        </p>

                        <div style='background: #dcfce7; border: 1px solid #bbf7d0;
                                    border-radius: 15px; padding: 20px;
                                    text-align: center; margin-bottom: 30px;'>
                            <span style='color: #166534; font-weight: 700; font-size: 0.95rem;'>
                                Tu contrasena ha sido actualizada correctamente.
                            </span>
                        </div>

                        <p style='color: #555; font-size: 1rem; line-height: 1.7; margin-bottom: 30px;'>
                            Si no has sido tu quien ha realizado este cambio, contacta
                            con el administrador del sistema de inmediato.
                        </p>

                        <div style='text-align: center; margin-bottom: 35px;'>
                            <a href='{Request.Scheme}://{Request.Host}'
                               style='display: inline-block; padding: 18px 40px;
                                      background: #1976d2; color: #ffffff;
                                      border-radius: 15px; text-decoration: none;
                                      font-weight: 700; font-size: 0.9rem;
                                      text-transform: uppercase; letter-spacing: 2px;
                                      box-shadow: 0 8px 20px rgba(25,118,210,0.2);'>
                                Ir al inicio de sesion
                            </a>
                        </div>

                        <p style='color: #bbb; font-size: 0.78rem; text-align: center;
                                  margin: 0; line-height: 1.6;'>
                            Este es un correo automatico, por favor no respondas a este mensaje.
                        </p>
                    </div>
                </div>"
            );

            Exito = true;
            return Page();
        }
    }
}