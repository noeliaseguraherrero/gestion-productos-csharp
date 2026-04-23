using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;
using System.Security.Claims;

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
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Producto.UsuarioId.ToString() != userId)
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var productoBorrar = await _context.Producto.FindAsync(Producto.Id);

            if (productoBorrar == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (productoBorrar.UsuarioId.ToString() != userId)
                return Forbid();

            // HISTORIAL — guardamos antes de borrar
            var historial = new HistorialAccion
            {
                Accion = "Eliminar",
                NombreProducto = productoBorrar.Nombre,
                CodigoProducto = productoBorrar.CodigoProducto,
                UsuarioId = int.Parse(userId),
                Fecha = DateTime.Now,
                Detalles = $"Precio: {productoBorrar.Precio}€ | Stock: {productoBorrar.Stock}"
            };
            _context.HistorialAcciones.Add(historial);
            await _context.SaveChangesAsync();

            _context.Producto.Remove(productoBorrar);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "El producto ha sido eliminado.";
            TempData["Tipo"] = "error";
            return RedirectToPage("./Inicio");
        }
    }
}