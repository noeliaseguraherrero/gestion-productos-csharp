using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Services;
using System.Net;
using System.Net.Mail;

namespace Practica1.Pages.Password
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ForgotPasswordModel(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public string? Mensaje { get; set; }
        public bool EsExito { get; set; } = false;  // <- AÑADIR

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == Email);

            // DEBUG - quita esto cuando funcione
            if (usuario == null)
            {
                Mensaje = "No se encontro ningun usuario con ese correo.";
                EsExito = false;
                return Page();
            }

            try
            {
                var token = Guid.NewGuid().ToString("N");
                usuario.ResetToken = token;
                usuario.ResetTokenExpiry = DateTime.Now.AddHours(1);
                await _context.SaveChangesAsync();

                var enlace = Url.Page(
                    "/Password/ResetPassword",
                    pageHandler: null,
                    values: new { token = token },
                    protocol: Request.Scheme
                );

                await _emailService.EnviarEmailAsync(
                    destinatario: usuario.Email!,
                    asunto: "Recuperación de contraseña - BecariosBD", cuerpoHtml: $@"
<div style='font-family: Inter, Arial, sans-serif; background-color: #f9f9f9;
            padding: 40px 20px; min-height: 100vh;'>
    <div style='max-width: 520px; margin: auto; background: #ffffff;
                border-radius: 30px; padding: 50px;
                box-shadow: 0 20px 60px rgba(0,0,0,0.08);
                border: 1px solid #f0f0f0;'>

        <h1 style='text-align: center; font-weight: 800; font-size: 1.8rem;
                   color: #1a1a1a; text-transform: uppercase;
                   letter-spacing: -1px; margin: 0 0 8px 0;'>
            Recuperar Acceso
        </h1>
        <p style='text-align: center; color: #999; font-size: 0.78rem;
                  font-weight: 700; letter-spacing: 2px;
                  text-transform: uppercase; margin: 0 0 40px 0;'>
            BecariosBD · Gestion Administrativa
        </p>

        <p style='color: #555; font-size: 1rem; line-height: 1.7; margin-bottom: 30px;'>
            Hemos recibido una solicitud para restablecer la contrasena de tu cuenta.
            Haz clic en el boton para continuar:
        </p>

        <div style='text-align: center; margin-bottom: 35px;'>
            <a href='{enlace}'
               style='display: inline-block; padding: 18px 40px;
                      background: #1976d2; color: #ffffff;
                      border-radius: 15px; text-decoration: none;
                      font-weight: 700; font-size: 0.9rem;
                      text-transform: uppercase; letter-spacing: 2px;
                      box-shadow: 0 8px 20px rgba(25,118,210,0.2);'>
                Restablecer Contrasena
            </a>
        </div>

        <div style='background: #fff8e1; border: 1px solid #ffe082;
                    border-radius: 12px; padding: 14px 18px;
                    text-align: center; margin-bottom: 25px;'>
            <span style='color: #f57f17; font-weight: 700; font-size: 0.85rem;'>
                Este enlace caduca en 1 hora
            </span>
        </div>

        <p style='color: #bbb; font-size: 0.78rem; text-align: center;
                  margin: 0; line-height: 1.6;'>
            Si no solicitaste este cambio, ignora este correo.<br>
            Tu contrasena no sera modificada.
        </p>
    </div>
</div>"
                );

                Mensaje = "Si ese correo existe, recibiras un enlace en breve.";
                EsExito = true;
            }
            catch (Exception ex)
            {
                // Verás el error exacto aquí
                Mensaje = $"Error al enviar el correo: {ex.Message}";
                EsExito = false;
            }

            return Page();
        }
    }
}
