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
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Producto.UsuarioId.ToString() != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Producto.Creador");

            if (!ModelState.IsValid)
                return Page();

            var productoDb = await _context.Producto
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == Producto.Id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (productoDb == null || (productoDb.UsuarioId.ToString() != userId && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            bool codigoExiste = await _context.Producto
                .AnyAsync(p => p.CodigoProducto == Producto.CodigoProducto
                            && p.Id != Producto.Id);

            if (codigoExiste)
            {
                ModelState.AddModelError("Producto.CodigoProducto",
                    "Ya existe un producto con ese código. Usa uno diferente.");
                return Page();
            }

            // Guardar valores anteriores para el historial
            var precioAnterior = productoDb.Precio;
            var stockAnterior = productoDb.Stock;

            Producto.UsuarioId = productoDb.UsuarioId;
            Producto.FechaCreacion = productoDb.FechaCreacion;
            _context.Attach(Producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // HISTORIAL
                // Cambia la sección del historial a esto:
                var historial = new HistorialAccion
                {
                    Accion = "Editar",
                    NombreProducto = Producto.Nombre,
                    CodigoProducto = Producto.CodigoProducto,
                    UsuarioId = int.Parse(userId),
                    Fecha = DateTime.Now,
                    // Quitamos el símbolo € y lo cambiamos por la palabra o nada
                    Detalles = $"Precio: {precioAnterior} -> {Producto.Precio} | " +
                               $"Stock: {stockAnterior} -> {Producto.Stock}"
                };
                _context.HistorialAcciones.Add(historial);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExiste(Producto.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Mensaje"] = "Cambios guardados correctamente.";
            TempData["Tipo"] = "info";
            return RedirectToPage("./Inicio");
        }

        private bool ProductoExiste(int id)
        {
            return _context.Producto.Any(p => p.Id == id);
        }
    }
}