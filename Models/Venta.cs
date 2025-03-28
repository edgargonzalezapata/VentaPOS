using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

[Table("Ventas")]
public partial class Venta
{
    public Venta()
    {
        DetallesVentas = new List<DetallesVenta>();
        Estado = "Pendiente"; // Valor por defecto
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VentaID { get; set; }

    public int UsuarioID { get; set; }

    public int? ClienteID { get; set; }

    [StringLength(50)]
    public string? NumeroFactura { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaVenta { get; set; }

    [StringLength(50)]
    public string? MetodoPago { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Impuestos { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Descuento { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Propina { get; set; }

    [StringLength(500)]
    public string? Comentarios { get; set; }

    [StringLength(20)]
    [Required]
    public string Estado { get; set; } = "Pendiente";

    [Required]
    [StringLength(20)]
    public string EmpresaRut { get; set; } = null!;

    public virtual Cliente? Cliente { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Empresa Empresa { get; set; } = null!;
    [InverseProperty("Venta")]
    public virtual ICollection<DetallesVenta> DetallesVentas { get; set; }
}
