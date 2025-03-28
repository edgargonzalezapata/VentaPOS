using System;
using System.Collections.Generic;
using VentaPOS.Models;

namespace VentaPOS.Models.ViewModels
{
    public class ReporteVentaVM
    {
        public int VentaID { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Impuestos { get; set; }
        public decimal? Descuento { get; set; }
        public string FormaPago { get; set; }
        public string Estado { get; set; }
        public Cliente Cliente { get; set; }
        public string UsuarioNombre { get; set; }
        public List<ProductoVendidoVM> ProductosVendidos { get; set; } = new List<ProductoVendidoVM>();
    }

    public class ProductoVendidoVM
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
    }
} 