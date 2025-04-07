using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public partial class Cliente
{
    public int ClienteId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Apellidos { get; set; }

    public string? Email { get; set; }

    public string? Telefono { get; set; }
    public string? RutCliente { get; set; }

    public DateTime? UltimaCompra { get; set; }

    [NotMapped]
    public int EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [Required]
    public string EmpresaRut { get; set; } = null!;

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
