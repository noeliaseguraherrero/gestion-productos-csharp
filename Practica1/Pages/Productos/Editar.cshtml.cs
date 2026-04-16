using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;
using System.Security.Claims;

namespace Practica1.Pages.Productos
{
    public class EditarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditarModel(ApplicationDbContext context)
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Producto.UsuarioId.ToString() != userId)
            {
                return Forbid();
            }

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

            var productoDb = await _context.Producto.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Producto.Id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (productoDb == null || productoDb.UsuarioId.ToString() != userId)
            {
                return Forbid();
            }

            // Mantenemos los datos originales de la DB que no están en el formulario
            Producto.UsuarioId = productoDb.UsuarioId;
            Producto.FechaCreacion = productoDb.FechaCreacion;

            _context.Attach(Producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExiste(Producto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Notificación de edición
            TempData["Mensaje"] = "Cambios guardados correctamente";
            TempData["Tipo"] = "info"; // Usamos 'info' o 'success'
            return RedirectToPage("./Inicio");
        }

        private bool ProductoExiste(int id)
        {
            return _context.Producto.Any(p => p.Id == id);
        }
    }
}