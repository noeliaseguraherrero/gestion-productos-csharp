using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practica1.Modelos

{
    [Table("user")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Column("user")]
        public string Nombre { get; set; }

        [Column("contraseña")]
        public string Contraseña { get; set; }

        // --- NUEVAS COLUMNAS ---
        [Column("rol")]
        public string Rol { get; set; } = "Usuario";

        [Column("activo")]
        public bool Activo { get; set; } = true;

        public virtual ICollection<Producto> Productos { get; set; }
    }
}