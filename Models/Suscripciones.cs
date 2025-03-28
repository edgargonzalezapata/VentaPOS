using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public class Suscripciones
{
    [Key]
    public int SuscripcionID { get; set; }

    [Required]
    [StringLength(20)]
    public string EmpresaRut { get; set; } = null!;

    [Required]
    public int PlanID { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    [StringLength(50)]
    public string? FormaPago { get; set; }

    public bool Pagado { get; set; }

    [StringLength(50)]
    public string? NumeroComprobante { get; set; }

    [Required]
    public bool Activa { get; set; }

    [Required]
    public int MaxUsuarios { get; set; }
    
    // Para compatibilidad durante la migración
    [NotMapped]
    public int? UsuarioAdminId { get; set; }
    
    // Para compatibilidad durante la migración
    [NotMapped]
    public int? EmpresaId { get; set; }

    // Propiedades de navegación
    [ForeignKey("PlanID")]
    public virtual Planes Plan { get; set; } = null!;

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
