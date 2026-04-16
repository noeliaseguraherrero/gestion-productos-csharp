using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;

namespace Practica1.Pages.Productos
{
    [Authorize]
    public class InicioModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InicioModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Producto> productos { get; set; }

        // 🚩 PROPIEDADES PARA LAS CARDS DE RESUMEN
        public int TotalProductos { get; set; }
        public decimal ValorInventario { get; set; }
        public int StockBajoCount { get; set; }

        // 📄 PROPIEDADES DE PAGINACIÓN Y BÚSQUEDA
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public string FiltroActual { get; set; }
        public const int RegistrosPorPagina = 10;

        // 👥 FILTRO DE USUARIOS (Corregido a List<Usuario>)
        public List<Modelos.Usuario> Usuarios { get; set; }
        public int? UsuarioSeleccionadoId { get; set; }

        public async Task OnGetAsync(int? pagina, string buscar, int? usuarioId)
        {
            PaginaActual = pagina ?? 1;
            FiltroActual = buscar;
            UsuarioSeleccionadoId = usuarioId;

            // 1. CARGAR USUARIOS 
            // Usamos la clase Usuario y el DbSet Usuarios definido en tu Contexto
            // Borra la línea anterior y escribe esto:
            Usuarios = await _context.Usuarios
                .OrderBy(u => u.Nombre) // Justo aquí, pulsa Ctrl + Espacio
                .ToListAsync();

            // 2. CONSULTA BASE
            IQueryable<Producto> consulta = _context.Producto.Include(p => p.Creador);

            // 3. FILTRO BUSCADOR (Nombre del producto)
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(p => p.Nombre.Contains(buscar));
            }

            // 4. FILTRO USUARIO (Por ID de autor)
            if (usuarioId.HasValue)
            {
                consulta = consulta.Where(p => p.UsuarioId == usuarioId.Value);
            }

            // 5. ESTADÍSTICAS (Calculadas sobre la consulta filtrada)
            // Usamos un conteo y suma directo a la base de datos para mejor rendimiento
            TotalProductos = await consulta.CountAsync();

            if (TotalProductos > 0)
            {
                ValorInventario = await consulta.SumAsync(p => p.Precio * p.Stock);
                StockBajoCount = await consulta.CountAsync(p => p.Stock < 10);
            }
            else
            {
                ValorInventario = 0;
                StockBajoCount = 0;
            }

            // 6. CÁLCULO DE PAGINACIÓN
            TotalPaginas = (int)Math.Ceiling(TotalProductos / (double)RegistrosPorPagina);

            if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;
            if (PaginaActual < 1) PaginaActual = 1;

            // 7. OBTENER PRODUCTOS PAGINADOS
            productos = await consulta
                .OrderByDescending(p => p.Id)
                .Skip((PaginaActual - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina)
                .ToListAsync();
        }
    }
}