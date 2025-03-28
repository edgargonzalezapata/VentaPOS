using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VentaPOS.Models
{
    public class BuscarUsuarioViewModel
    {
        [Required(ErrorMessage = "El término de búsqueda es requerido")]
        [Display(Name = "Buscar usuario por nombre o correo")]
        public string TerminoBusqueda { get; set; } = string.Empty;
        
        public List<Usuario> UsuariosEncontrados { get; set; } = new List<Usuario>();
    }
} 