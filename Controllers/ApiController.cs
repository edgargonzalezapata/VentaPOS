using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VentaPOS.Data;
using VentaPOS.Models;
using VentaPOS.Models.ViewModels;
using System.Linq;

namespace VentaPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasApiController : ControllerBase
    {
        private readonly VentaPosContext _context;

        public VentasApiController(VentaPosContext context)
        {
            _context = context;
        }

        [HttpGet("Detalles/{id}")]
        public async Task<IActionResult> GetDetalles(int id)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return Unauthorized();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.VentaID == id && v.EmpresaRut == empresaRut);

            if (venta == null)
            {
                return NotFound();
            }

            var detallesVenta = new ReporteVentaVM
            {
                VentaID = venta.VentaID,
                Fecha = venta.FechaVenta,
                Cliente = venta.Cliente,
                Total = venta.Total,
                Impuestos = venta.Impuestos,
                Descuento = venta.Descuento,
                ProductosVendidos = venta.DetallesVentas.Select(d => new ProductoVendidoVM
                {
                    ProductoID = d.ProductoID,
                    Nombre = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };

            return Ok(detallesVenta);
        }
    }
} 