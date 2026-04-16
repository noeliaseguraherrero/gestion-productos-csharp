using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;

namespace Practica1.Pages.Productos
{
    public class DetalleModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetalleModel(ApplicationDbContext context)
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

            return Page();
        }
    }
}
