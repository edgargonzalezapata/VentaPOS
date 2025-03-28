using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    [Required]
    public string Nombre { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    [Required]
    public byte[] Contraseña { get; set; } = null!;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public bool Activo { get; set; } = true;

    [Required]
    [Display(Name = "RUT de la Empresa")]
    public string EmpresaRut { get; set; } = null!;

    [NotMapped]
    public int EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;

    public virtual ICollection<Suscripciones> Suscripciones { get; set; } = new List<Suscripciones>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();

    public virtual ICollection<Roles> Rols { get; set; } = new List<Roles>();
}
