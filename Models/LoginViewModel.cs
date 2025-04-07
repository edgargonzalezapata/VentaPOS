using System.ComponentModel.DataAnnotations;

namespace VentaPOS.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrónico o RUT es requerido")]
        [Display(Name = "Correo electrónico o RUT")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
        
        [Display(Name = "Tipo de Usuario")]
        public string TipoUsuario { get; set; } = "Usuario"; // Valores: "Empresa" o "Usuario"

        [Display(Name = "Etiqueta de Entrada")]
        public string EtiquetaEntrada => TipoUsuario == "Empresa" ? "RUT" : "Correo Electrónico";
    }
} 