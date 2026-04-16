using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Practica1.Datos;
using Practica1.Modelos;
using System.Security.Claims;

namespace Practica1.Pages.Productos
{
    public class CrearModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CrearModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Producto Producto { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ?? EL CAMBIO: Eliminamos el objeto Creador de la validación
            ModelState.Remove("Producto.Creador");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                Producto.UsuarioId = int.Parse(userId);
                Producto.FechaCreacion = DateTime.Now;

                _context.Producto.Add(Producto);
                await _context.SaveChangesAsync();

                // Notificación de éxito
                TempData["Mensaje"] = "¡Producto creado con éxito!";
                TempData["Tipo"] = "success";
                return RedirectToPage("./Inicio");
            }

            return Page();
        }
    }
}