using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;
using System.Security.Claims;

namespace Practica1.Pages.Productos
{
    [Authorize]
    public class ImportarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ImportarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile FileCsv { get; set; }

        // Resultados
        public bool MostrarResultados { get; set; } = false;
        public int Creados { get; set; } = 0;
        public int Actualizados { get; set; } = 0;
        public List<ErrorImportacion> Errores { get; set; } = new();

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (FileCsv == null || FileCsv.Length == 0)
            {
                ModelState.AddModelError("", "Selecciona un fichero CSV.");
                return Page();
            }

            if (!FileCsv.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "El fichero debe ser .csv");
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Forbid();

            // Leer todas las líneas del CSV
            var lineas = new List<string>();
            using (var reader = new StreamReader(FileCsv.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var linea = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(linea))
                        lineas.Add(linea);
                }
            }

            // Saltar la cabecera
            var filas = lineas.Skip(1).ToList();

            // Cargar todos los códigos existentes en memoria para comparar
            var productosExistentes = await _context.Producto
                .ToDictionaryAsync(p => p.CodigoProducto.ToLower());

            int numeroLinea = 1; // empieza en 1 porque saltamos la cabecera

            foreach (var fila in filas)
            {
                numeroLinea++;
                var columnas = fila.Split(',');

                // Validar número de columnas
                if (columnas.Length < 4)
                {
                    Errores.Add(new ErrorImportacion
                    {
                        Linea = numeroLinea,
                        Codigo = "-",
                        Nombre = "-",
                        Motivo = "Formato incorrecto: faltan columnas"
                    });
                    continue;
                }

                var codigo = columnas[0].Trim();
                var nombre = columnas[1].Trim();
                var precioStr = columnas[2].Trim();
                var stockStr = columnas[3].Trim();

                // Validar campos vacíos
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nombre))
                {
                    Errores.Add(new ErrorImportacion
                    {
                        Linea = numeroLinea,
                        Codigo = codigo,
                        Nombre = nombre,
                        Motivo = "Código o nombre vacío"
                    });
                    continue;
                }

                // Validar precio
                if (!decimal.TryParse(precioStr,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal precio) || precio < 0 || precio > 1000)
                {
                    Errores.Add(new ErrorImportacion
                    {
                        Linea = numeroLinea,
                        Codigo = codigo,
                        Nombre = nombre,
                        Motivo = $"Precio inválido: '{precioStr}' (debe ser 0-1000)"
                    });
                    continue;
                }

                // Validar stock
                if (!int.TryParse(stockStr, out int stock) || stock < 0)
                {
                    Errores.Add(new ErrorImportacion
                    {
                        Linea = numeroLinea,
                        Codigo = codigo,
                        Nombre = nombre,
                        Motivo = $"Stock inválido: '{stockStr}'"
                    });
                    continue;
                }

                // ¿Existe ya en la BD?
                if (productosExistentes.TryGetValue(codigo.ToLower(), out var productoExistente))
                {
                    // ACTUALIZAR
                    productoExistente.Nombre = nombre;
                    productoExistente.Precio = precio;
                    productoExistente.Stock = stock;
                    _context.Producto.Update(productoExistente);
                    Actualizados++;
                }
                else
                {
                    // CREAR
                    var nuevo = new Producto
                    {
                        CodigoProducto = codigo,
                        Nombre = nombre,
                        Precio = precio,
                        Stock = stock,
                        FechaCreacion = DateTime.Now,
                        UsuarioId = int.Parse(userId)
                    };
                    _context.Producto.Add(nuevo);
                    Creados++;
                }
            }

            await _context.SaveChangesAsync();
            MostrarResultados = true;

            return Page();
        }
    }

    // Clase auxiliar para los errores
    public class ErrorImportacion
    {
        public int Linea { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Motivo { get; set; }
    }
}