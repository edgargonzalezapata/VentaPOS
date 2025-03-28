using System;
using System.Collections.Generic;
using VentaPOS.Models;

namespace VentaPOS.Models.ViewModels
{
    public class ReporteVentasViewModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal PromedioVenta { get; set; }
        public List<Venta> Ventas { get; set; }
        public List<ProductoVendido> ProductosVendidos { get; set; }
    }

    public class ProductoVendido
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
} 