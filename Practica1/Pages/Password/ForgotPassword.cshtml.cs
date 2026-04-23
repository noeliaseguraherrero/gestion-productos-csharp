using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using System.Net;
using System.Net.Mail;

namespace Practica1.Pages.Password
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ForgotPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public string? Mensaje { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Buscar usuario por email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == Email);

            // Siempre mostramos el mismo mensaje por seguridad
            // (no revelar si el email existe o no)
            Mensaje = "Si ese correo existe, recibirás un enlace en breve.";

            if (usuario == null)
                return Page();

            // 2. Generar token único y fecha de expiración (1 hora)
            var token = Guid.NewGuid().ToString("N");
            usuario.ResetToken = token;
            usuario.ResetTokenExpiry = DateTime.Now.AddHours(1);
            await _context.SaveChangesAsync();

            // 3. Construir el enlace
            var enlace = Url.Page(
                "/Password/ResetPassword",
                pageHandler: null,
                values: new { token = token },
                protocol: Request.Scheme
            );

            // 4. Enviar el correo con SmtpClient
            await EnviarEmailAsync(usuario.Email, enlace!);

            return Page();
        }

        private async Task EnviarEmailAsync(string destinatario, string enlace)
        {
            var remitente = "tucorreo@gmail.com";       // <-- Tu email
            var password = "tu_contraseña_de_app";     // <-- Tu contraseña de app

            var mensaje = new MailMessage
            {
                From = new MailAddress(remitente, "Gestión de Productos"),
                Subject = "Recuperación de contraseña",
                Body = $@"
                    <h3>Recuperación de contraseña</h3>
                    <p>Haz clic en el enlace para restablecer tu contraseña:</p>
                    <a href='{enlace}'>Restablecer contraseña</a>
                    <p>Este enlace caduca en <strong>1 hora</strong>.</p>
                    <p>Si no solicitaste esto, ignora este correo.</p>",
                IsBodyHtml = true
            };
            mensaje.To.Add(destinatario);

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(remitente, password)
            };

            await smtp.SendMailAsync(mensaje);
        }
    }
}
