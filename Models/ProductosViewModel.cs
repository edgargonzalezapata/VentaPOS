using System.Collections.Generic;

namespace VentaPOS.Models
{
    public class ProductosViewModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public Usuario? Usuario { get; set; }
    }
} 