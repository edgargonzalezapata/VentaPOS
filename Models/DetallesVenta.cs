using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VentaPOS.Models;

namespace VentaPOS.Models
{
    public class DetallesVenta
    {
        [Key]
        [Column("DetalleID")]
        public int DetalleID { get; set; }

        [ForeignKey("Venta")]
        [Column("VentaID")]
        public int VentaID { get; set; }

        [ForeignKey("Producto")]
        [Column("ProductoID")]
        public int ProductoID { get; set; }
        
        public int Cantidad { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Descuento { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Impuesto { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [ForeignKey("VentaID")]
        public virtual Venta Venta { get; set; }

        [ForeignKey("ProductoID")]
        public virtual Producto Producto { get; set; }
    }
} 