using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;
using System.Security.Claims; // ?? Necesario

namespace Practica1.Pages.Productos
{
    public class BorrarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public BorrarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Producto Producto { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Producto = await _context.Producto.FirstOrDefaultAsync(p => p.Id == id);

            if (Producto == null)
            {
                return NotFound();
            }

            // ?? SEGURIDAD: Validar que el usuario es el creador
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Producto.UsuarioId.ToString() != userId)
            {
                return Forbid(); // O RedirectToPage("./Inicio")
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Buscamos el producto real en la base de datos
            var productoBorrar = await _context.Producto.FindAsync(Producto.Id);

            if (productoBorrar == null)
            {
                return NotFound();
            }

            // ?? SEGURIDAD: Doble verificación antes de borrar
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (productoBorrar.UsuarioId.ToString() != userId)
            {
                return Forbid();
            }

            _context.Producto.Remove(productoBorrar);
            await _context.SaveChangesAsync();

            // Notificación de borrado
            TempData["Mensaje"] = "El producto ha sido eliminado";
            TempData["Tipo"] = "error"; // El color rojo de 'error' queda bien para eliminaciones
            return RedirectToPage("./Inicio");
        }
    }
}