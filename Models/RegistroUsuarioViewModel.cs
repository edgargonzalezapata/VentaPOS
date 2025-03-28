using System.ComponentModel.DataAnnotations;

namespace VentaPOS.Models
{
    public class RegistroUsuarioViewModel
    {
        [Required(ErrorMessage = "El RUT es requerido")]
        [Display(Name = "RUT de la Empresa")]
        [StringLength(20, ErrorMessage = "El RUT no puede tener más de 20 caracteres")]
        public string? Rut { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre de la Empresa")]
        public string? NombreEmpresa { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido")]
        [Display(Name = "Correo Electrónico")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "Formato de teléfono no válido")]
        public string? Telefono { get; set; }
        
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        // Información del administrador
        [Required(ErrorMessage = "El nombre del administrador es requerido")]
        [Display(Name = "Nombre del Administrador")]
        public string? NombreAdmin { get; set; }
        
        [Display(Name = "Plan Seleccionado")]
        public string? PlanSeleccionado { get; set; }

        [Display(Name = "ID de Usuario")]
        public int? UsuarioId { get; set; }
    }
} 