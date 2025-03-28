using System.Collections.Generic;
using VentaPOS.Models;

namespace VentaPOS.Models.ViewModels
{
    public class ProductosIndexVM
    {
        public List<Producto> Productos { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
} 