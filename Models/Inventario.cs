using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public partial class Inventario
{
    public int InventarioId { get; set; }

    [Column("ProductoID")]
    public int ProductoID { get; set; }

    public int Cantidad { get; set; }

    public string? Ubicacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    [NotMapped]
    public int? EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [Required]
    public string EmpresaRut { get; set; } = null!;

    [ForeignKey("ProductoID")]
    public Producto Producto { get; set; } = null!;

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;
}
