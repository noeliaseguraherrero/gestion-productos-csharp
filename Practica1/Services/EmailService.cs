using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Practica1.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtp;

        public EmailService(IOptions<SmtpSettings> smtp)
        {
            _smtp = smtp.Value;
        }

        public async Task EnviarEmailAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var mensaje = new MailMessage
            {
                From = new MailAddress(_smtp.Usuario, "BecariosBD"),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };
            mensaje.To.Add(destinatario);

            using var smtp = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                EnableSsl = _smtp.EnableSsl,
                Credentials = new NetworkCredential(_smtp.Usuario, _smtp.Password)
            };

            await smtp.SendMailAsync(mensaje);
        }
    }
}
