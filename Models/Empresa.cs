using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public partial class Empresa
{
    [Key]
    [Display(Name = "RUT")]
    [Required(ErrorMessage = "El RUT es requerido")]
    [StringLength(20, ErrorMessage = "El RUT no puede tener más de 20 caracteres")]
    public string Rut { get; set; } = null!;

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre es requerido")]
    public string NombreEmpresa { get; set; } = null!;

    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [Display(Name = "Fecha de Registro")]
    public DateTime? FechaRegistro { get; set; }

    [Display(Name = "Activo")]
    public bool? Activo { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [Display(Name = "Contraseña")]
    public byte[] Contraseña { get; set; } = null!;

    [NotMapped]
    public int EmpresaId { get; set; }  // Para compatibilidad durante la migración

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<ConfiguracionEmpresa> ConfiguracionEmpresas { get; set; } = new List<ConfiguracionEmpresa>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
    
    public virtual ICollection<Suscripciones> Suscripciones { get; set; } = new List<Suscripciones>();
}
