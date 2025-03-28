using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VentaPOS.Models.ViewModels
{
    public class VentaDTO
    {
        public int? ClienteID { get; set; }
        public int UsuarioID { get; set; }
        public string EmpresaRut { get; set; }
        public DateTime FechaVenta { get; set; }
        public string MetodoPago { get; set; }
        public string Comentarios { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaDTO> DetallesVenta { get; set; } = new List<DetalleVentaDTO>();
    }

    public class DetalleVentaDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
} 