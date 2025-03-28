using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentaPOS.Models;

public class Planes
{
    [Key]
    public int PlanID { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Nombre del Plan")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Máximo de Usuarios")]
    public int? MaxUsuarios { get; set; }

    [Display(Name = "Precio")]
    public int? Precio { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Suscripciones> Suscripciones { get; set; } = new List<Suscripciones>();
} 