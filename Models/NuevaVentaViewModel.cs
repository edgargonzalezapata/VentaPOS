using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VentaPOS.Models
{
    public class NuevaVentaViewModel
    {
        public string? ClienteId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public string? MetodoPago { get; set; }
        public string? Notas { get; set; }
        public List<DetalleVentaViewModel> DetallesVenta { get; set; } = new List<DetalleVentaViewModel>();
    }

    public class DetalleVentaViewModel
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
} 