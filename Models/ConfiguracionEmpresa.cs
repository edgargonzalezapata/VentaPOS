using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public partial class ConfiguracionEmpresa
{
    public int ConfiguracionId { get; set; }

    [NotMapped]
    public int EmpresaId { get; set; }  // Para compatibilidad durante la migración

    [Required]
    public string EmpresaRut { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public string? Descripcion { get; set; }

    [ForeignKey("EmpresaRut")]
    public virtual Empresa Empresa { get; set; } = null!;
}
