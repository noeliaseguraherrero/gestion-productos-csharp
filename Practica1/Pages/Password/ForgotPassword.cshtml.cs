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
                    destinatario: usuario.Email!, asunto: "Recuperacion de contrasena - BecariosBD",
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
                Recuperar contrasena
            </h2>
            <p style='color:#64748b;font-size:0.88rem;line-height:1.7;margin:0 0 28px 0;'>
                Hemos recibido una solicitud para restablecer la contrasena de tu cuenta.
                Sigue estos pasos:
            </p>

            <!-- PASOS -->
            <div style='margin-bottom:28px;'>
                <div style='display:flex;align-items:center;gap:12px;margin-bottom:12px;'>
                    <span style='width:22px;height:22px;border-radius:50%;background:#1d4ed8;
                                 color:#fff;font-size:0.7rem;font-weight:800;
                                 display:inline-flex;align-items:center;
                                 justify-content:center;flex-shrink:0;'>1</span>
                    <span style='color:#475569;font-size:0.85rem;'>Haz clic en el boton de abajo</span>
                </div>
                <div style='display:flex;align-items:center;gap:12px;margin-bottom:12px;'>
                    <span style='width:22px;height:22px;border-radius:50%;background:#1d4ed8;
                                 color:#fff;font-size:0.7rem;font-weight:800;
                                 display:inline-flex;align-items:center;
                                 justify-content:center;flex-shrink:0;'>2</span>
                    <span style='color:#475569;font-size:0.85rem;'>Escribe tu nueva contrasena</span>
                </div>
                <div style='display:flex;align-items:center;gap:12px;'>
                    <span style='width:22px;height:22px;border-radius:50%;background:#1d4ed8;
                                 color:#fff;font-size:0.7rem;font-weight:800;
                                 display:inline-flex;align-items:center;
                                 justify-content:center;flex-shrink:0;'>3</span>
                    <span style='color:#475569;font-size:0.85rem;'>Accede normalmente al sistema</span>
                </div>
            </div>

            <!-- BOTON -->
            <div style='text-align:center;margin-bottom:25px;'>
                <a href='{enlace}'
                   style='display:inline-block;padding:14px 36px;
                          background:#1d4ed8;color:#fff;border-radius:10px;
                          text-decoration:none;font-weight:700;font-size:0.85rem;
                          text-transform:uppercase;letter-spacing:1.5px;'>
                    Restablecer Contrasena
                </a>
            </div>

            <!-- AVISO -->
            <p style='color:#94a3b8;font-size:0.78rem;text-align:center;
                      margin:0;line-height:1.7;border-top:1px solid #f1f5f9;padding-top:20px;'>
                Este enlace caduca en <strong style='color:#c2410c;'>1 hora</strong>.<br>
                Si no solicitaste este cambio, ignora este correo.
            </p>
        </div>
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
