using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

[Table("Roles")]  // Mapear explícitamente a la tabla Roles
public class Roles
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RolId { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(200)]
    public string? Descripcion { get; set; }

    [NotMapped]
    public int? EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [Required]
    [StringLength(20)]
    public string EmpresaRut { get; set; } = null!;

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
