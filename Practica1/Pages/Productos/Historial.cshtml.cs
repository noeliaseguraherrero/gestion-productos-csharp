using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;

namespace Practica1.Pages.Productos
{
    [Authorize(Roles = "Admin")]
    public class HistorialModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistorialModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<HistorialAccion> Historial { get; set; }
        public List<Usuario> Usuarios { get; set; }

        // Filtros
        public string FiltroAccion { get; set; }
        public int? FiltroUsuarioId { get; set; }
        public string FiltroFecha { get; set; }

        // Paginación
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public const int RegistrosPorPagina = 15;

        public async Task OnGetAsync(int? pagina, string accion, int? usuarioId, string fecha)
        {
            PaginaActual = pagina ?? 1;
            FiltroAccion = accion;
            FiltroUsuarioId = usuarioId;
            FiltroFecha = fecha;

            Usuarios = await _context.Usuarios.OrderBy(u => u.Nombre).ToListAsync();

            IQueryable<HistorialAccion> consulta = _context.HistorialAcciones
                .Include(h => h.Usuario);

            if (!string.IsNullOrEmpty(accion))
                consulta = consulta.Where(h => h.Accion == accion);

            if (usuarioId.HasValue)
                consulta = consulta.Where(h => h.UsuarioId == usuarioId.Value);

            if (!string.IsNullOrEmpty(fecha) && DateTime.TryParse(fecha, out DateTime fechaParsed))
                consulta = consulta.Where(h => h.Fecha.Date == fechaParsed.Date);

            TotalRegistros = await consulta.CountAsync();
            TotalPaginas = (int)Math.Ceiling(TotalRegistros / (double)RegistrosPorPagina);

            if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;
            if (PaginaActual < 1) PaginaActual = 1;

            Historial = await consulta
                .OrderByDescending(h => h.Fecha)
                .Skip((PaginaActual - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina)
                .ToListAsync();

        }
    }
}