using System.Collections.Generic;

namespace VentaPOS.Models.ViewModels
{
    public class ProductoRequest
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }

    public class NuevoVentaRequest
    {
        public string? ClienteId { get; set; }
        public string TipoPedido { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? Impuestos { get; set; }
        public decimal? Descuento { get; set; }
        public decimal Total { get; set; }
        public List<ProductoRequest> Productos { get; set; } = new List<ProductoRequest>();
    }
} 