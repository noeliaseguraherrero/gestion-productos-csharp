using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practica1.Modelos
{
    public class HistorialAccion
    {
        [Key]
        public int Id { get; set; }

        // Qué se hizo: "Crear", "Editar", "Eliminar"
        public string Accion { get; set; }

        // Sobre qué producto
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }

        // Quién lo hizo
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        // Cuándo
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Detalles extra (precio anterior, stock, etc.)
        public string? Detalles { get; set; }
    }
}