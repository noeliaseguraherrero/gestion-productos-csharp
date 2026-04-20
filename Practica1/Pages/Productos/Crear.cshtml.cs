using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
            ModelState.Remove("Producto.Creador");

            if (!ModelState.IsValid)
                return Page();

            // Comprobar si ya existe el código
            bool codigoExiste = await _context.Producto
                .AnyAsync(p => p.CodigoProducto == Producto.CodigoProducto);

            if (codigoExiste)
            {
                ModelState.AddModelError("Producto.CodigoProducto",
                    "Ya existe un producto con ese código. Usa uno diferente.");
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                Producto.UsuarioId = int.Parse(userId);
                Producto.FechaCreacion = DateTime.Now;
                _context.Producto.Add(Producto);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "¡Producto creado con éxito!";
                TempData["Tipo"] = "success";
                return RedirectToPage("./Inicio");
            }

            return Page();
        }
    }
}