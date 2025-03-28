using System.Collections.Generic;

namespace VentaPOS.Models.ViewModels
{
    public class PanelVentasViewModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public Usuario Usuario { get; set; } = null!;
    }
} 