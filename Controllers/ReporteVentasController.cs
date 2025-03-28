using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentaPOS.Data;
using VentaPOS.Models;
using VentaPOS.Models.ViewModels;

namespace VentaPOS.Controllers
{
    [Authorize]
    public class ReporteVentasController : Controller
    {
        private readonly VentaPosContext _context;

        public ReporteVentasController(VentaPosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? clienteID)
        {
            // Establecer valores por defecto si no vienen en la solicitud
            fechaInicio ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            fechaFin ??= DateTime.Now.Date.AddDays(1).AddTicks(-1); // Hasta el final del día actual

            // Cargar lista de clientes para el filtro
            ViewBag.Clientes = await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync();

            // Consulta base
            var query = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin);

            // Aplicar filtro por cliente si se especifica
            if (clienteID.HasValue)
            {
                query = query.Where(v => v.ClienteID == clienteID.Value);
            }

            // Ejecutar la consulta con proyección para manejar valores NULL
            var ventas = await query.Select(v => new
            {
                v.VentaID,
                v.FechaVenta,
                v.Total,
                v.MetodoPago,
                v.Estado,
                Impuestos = v.Impuestos,
                Descuento = v.Descuento,
                Subtotal = v.Subtotal,
                v.Cliente,
                v.Usuario,
                DetallesVentas = v.DetallesVentas.Select(d => new
                {
                    d.ProductoID,
                    d.Producto.Nombre,
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.Subtotal
                }).ToList()
            }).ToListAsync();

            // Mapear a ViewModel
            var reporteVMs = ventas.Select(v => new ReporteVentaVM
            {
                VentaID = v.VentaID,
                Fecha = v.FechaVenta,
                Total = v.Total,
                FormaPago = v.MetodoPago ?? "No especificado",
                Estado = v.Estado,
                Impuestos = v.Impuestos,
                Descuento = v.Descuento,
                Subtotal = v.Subtotal,
                Cliente = v.Cliente,
                UsuarioNombre = v.Usuario?.Nombre ?? "No especificado",
                ProductosVendidos = v.DetallesVentas.Select(d => new ProductoVendidoVM
                {
                    ProductoID = d.ProductoID,
                    Nombre = d.Nombre,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            }).ToList();

            // Pasar los parámetros de filtro a la vista para mantenerlos
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.ClienteID = clienteID;

            return View(reporteVMs);
        }

        public async Task<IActionResult> DetallesVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.VentaID == id);

            if (venta == null)
            {
                return NotFound();
            }

            var detalleVM = new ReporteVentaVM
            {
                VentaID = venta.VentaID,
                Fecha = venta.FechaVenta,
                Total = venta.Total,
                Impuestos = venta.Impuestos,
                Descuento = venta.Descuento,
                Cliente = venta.Cliente,
                ProductosVendidos = venta.DetallesVentas.Select(d => new ProductoVendidoVM
                {
                    ProductoID = d.ProductoID,
                    Nombre = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };

            return PartialView("_DetallesVenta", detalleVM);
        }
    }
} 