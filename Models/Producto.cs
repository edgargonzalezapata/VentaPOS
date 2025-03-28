using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

[Table("Productos")]
public partial class Producto
{
    [Key]
    [Column("ProductoID")]
    public int ProductoID { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500)]
    [Column(TypeName = "nvarchar(500)")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
    [Column(TypeName = "decimal(18,0)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    [Column("CategoriaID")]
    public int CategoriaID { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    [NotMapped]
    public int? EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [Required]
    public string EmpresaRut { get; set; } = null!;

    [NotMapped]
    public int CantidadVendida { get; set; }

    [ForeignKey("CategoriaID")]
    public virtual Categoria Categoria { get; set; }

    [ForeignKey("EmpresaRut")]
    public virtual Empresa? Empresa { get; set; }

    public virtual ICollection<DetallesVenta> DetallesVentas { get; set; } = new List<DetallesVenta>();

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
}
