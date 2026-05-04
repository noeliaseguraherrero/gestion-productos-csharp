using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; // <-- AÑADIR

namespace Practica1.Modelos
{
    [Index(nameof(CodigoProducto), IsUnique = true)] // <-- AÑADIR
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código de producto es obligatorio")]
        public string CodigoProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Range(typeof(decimal), "0", "10000", ErrorMessage = "El precio debe estar entre 0 y 10000 usd")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; }
        public DateTime FechaCreacion { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Creador { get; set; }
    }
}