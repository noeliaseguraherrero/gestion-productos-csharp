using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Necesario para [ForeignKey]

namespace Practica1.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Range(0, 1000, ErrorMessage = "El precio debe estar entre 0 y 1000 usd")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public DateTime FechaCreacion { get; set; }

        // --- NUEVAS COLUMNAS PARA EL CREADOR ---

        [Required]
        public int UsuarioId { get; set; } // Esta es la columna física en la DB

        [ForeignKey("UsuarioId")]
        public virtual Usuario Creador { get; set; } // Esta es la propiedad de navegación
    }
}