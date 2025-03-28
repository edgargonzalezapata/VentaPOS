using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public partial class Categoria
{
    [Key]
    [Column("CategoriaID")]
    public int CategoriaID { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
    public string? Descripcion { get; set; }

    [NotMapped]
    public int? EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [Required]
    public string EmpresaRut { get; set; } = null!;

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
