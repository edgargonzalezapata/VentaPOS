using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentaPOS.Data;
using VentaPOS.Models;
using VentaPOS.Models.ViewModels;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data.Common;
using VentaPOS.Extensions;
using System.Text.Json;

namespace VentaPOS.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly VentaPosContext _context;
        private readonly ILogger<VentasController> _logger;

        public VentasController(VentaPosContext context, ILogger<VentasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
                if (string.IsNullOrEmpty(usuarioIdString))
                {
                    return RedirectToAction("Login", "Usuarios");
                }

                if (!int.TryParse(usuarioIdString, out int usuarioId))
                {
                    return RedirectToAction("Login", "Usuarios");
                }

                var usuario = await _context.Usuarios
                    .Include(u => u.Empresa)
                    .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Usuarios");
                }

                var viewModel = new PanelVentasViewModel
                {
                    Productos = await _context.Productos
                        .Include(p => p.Categoria)
                        .Where(p => p.EmpresaRut == usuario.EmpresaRut)
                        .ToListAsync(),
                    Clientes = await _context.Clientes
                        .Where(c => c.EmpresaRut == usuario.EmpresaRut)
                        .ToListAsync(),
                    Usuario = usuario
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la vista de ventas");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        // GET: Ventas/NuevaVenta
        public async Task<IActionResult> NuevaVenta()
        {
            // Verificar si hay una sesión activa
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (!int.TryParse(usuarioIdString, out int usuarioId))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Cargar listas para los dropdowns
            ViewData["Clientes"] = new SelectList(await _context.Clientes
                .Where(c => c.EmpresaRut == empresaRut)
                .OrderBy(c => c.Nombre)
                .ToListAsync(), "ClienteId", "Nombre");

            ViewData["Productos"] = await _context.Productos
                .Where(p => p.EmpresaRut == empresaRut)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            var viewModel = new NuevaVentaViewModel
            {
                FechaVenta = DateTime.Now,
                UsuarioId = usuarioId.ToString()
            };

            return View(viewModel);
        }

        // POST: Ventas/NuevaVenta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevaVenta(NuevaVentaViewModel model)
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!int.TryParse(usuarioIdString, out int usuarioId))
                    {
                        return RedirectToAction("Login", "Usuarios");
                    }

                    // Crear la venta
                    var venta = new Venta
                    {
                        ClienteID = string.IsNullOrEmpty(model.ClienteId) ? null : int.Parse(model.ClienteId),
                        UsuarioID = usuarioId,
                        FechaVenta = model.FechaVenta,
                        Total = model.DetallesVenta.Sum(d => d.Cantidad * d.PrecioUnitario),
                        Comentarios = model.Notas,
                        MetodoPago = model.MetodoPago ?? "Normal",
                        EmpresaRut = empresaRut
                    };

                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();

                    // Crear los detalles de la venta
                    foreach (var detalle in model.DetallesVenta)
                    {
                        if (int.TryParse(detalle.ProductoId, out int productoId))
                        {
                            var detalleVenta = new DetallesVenta
                            {
                                ProductoID = productoId,
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario
                            };

                            _context.DetallesVenta.Add(detalleVenta);

                            // Actualizar el inventario del producto
                            var inventario = await _context.Inventarios
                                .FirstOrDefaultAsync(i => i.ProductoID == productoId && i.EmpresaRut == empresaRut);
                            
                            if (inventario != null)
                            {
                                inventario.Cantidad -= detalle.Cantidad;
                                _context.Update(inventario);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "Venta registrada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al registrar la venta: {ex.Message}");
                }
            }

            // Si llegamos aquí, algo falló, volvemos a cargar los datos para el formulario
            ViewData["Clientes"] = new SelectList(await _context.Clientes
                .Where(c => c.EmpresaRut == empresaRut)
                .OrderBy(c => c.Nombre)
                .ToListAsync(), "ClienteId", "Nombre", model.ClienteId);

            ViewData["Productos"] = await _context.Productos
                .Where(p => p.EmpresaRut == empresaRut)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPedido([FromBody] NuevoVentaRequest ventaParams)
        {
            try
            {
                var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
                if (string.IsNullOrEmpty(usuarioIdString))
                {
                    return Json(new { success = false, message = "Sesión no válida" });
                }

                if (!int.TryParse(usuarioIdString, out int usuarioId))
                {
                    return Json(new { success = false, message = "ID de usuario no válido" });
                }

                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return Json(new { success = false, message = "Empresa no válida" });
                }

                // Calcular el subtotal
                decimal subtotal = ventaParams.Subtotal;
                
                // Manejar el ID del cliente - convertir de string a int?
                int? clienteId = null;
                if (!string.IsNullOrEmpty(ventaParams.ClienteId) && int.TryParse(ventaParams.ClienteId, out int parsedClienteId) && parsedClienteId > 0)
                {
                    clienteId = parsedClienteId;
                }

                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insertar la venta
                            string insertVentaSql = @"
                                INSERT INTO Ventas (UsuarioID, ClienteID, FechaVenta, MetodoPago, Subtotal, Impuestos, Descuento, Total, Comentarios, EmpresaRut, Estado)
                                VALUES (@UsuarioID, @ClienteID, @FechaVenta, @MetodoPago, @Subtotal, @Impuestos, @Descuento, @Total, @Comentarios, @EmpresaRut, @Estado);
                                SELECT CAST(SCOPE_IDENTITY() as int)";

                            var ventaId = await connection.ExecuteScalarAsync<int>(insertVentaSql, new
                            {
                                UsuarioID = usuarioId,
                                ClienteID = clienteId,
                                FechaVenta = DateTime.Now,
                                MetodoPago = ventaParams.TipoPedido,
                                Subtotal = ventaParams.Subtotal,
                                Impuestos = ventaParams.Impuestos,
                                Descuento = ventaParams.Descuento,
                                Total = ventaParams.Total,
                                Comentarios = ventaParams.Notas,
                                EmpresaRut = empresaRut,
                                Estado = "Pendiente" // Estado por defecto para nuevos pedidos
                            }, transaction);

                            // Insertar los detalles de la venta
                            foreach (var producto in ventaParams.Productos)
                            {
                                // Convertir ProductoId de string a int
                                if (!int.TryParse(producto.ProductoId, out int productoId))
                                {
                                    throw new Exception($"ID de producto no válido: {producto.ProductoId}");
                                }

                                // Calcular el subtotal para este detalle
                                decimal subtotalDetalle = producto.Cantidad * producto.Precio;
                                
                                // Por ahora, no aplicamos descuentos a nivel de producto individual
                                decimal? descuentoProducto = null;
                                
                                // Calcular el impuesto para este producto (19%)
                                decimal? impuestoProducto = subtotalDetalle * 0.19m;

                                string insertDetalleSql = @"
                                    INSERT INTO DetallesVenta (VentaID, ProductoID, Cantidad, PrecioUnitario, Descuento, Impuesto, Subtotal)
                                    VALUES (@VentaID, @ProductoID, @Cantidad, @PrecioUnitario, @Descuento, @Impuesto, @Subtotal)";

                                await connection.ExecuteAsync(insertDetalleSql, new
                                {
                                    VentaID = ventaId,
                                    ProductoID = productoId,
                                    Cantidad = producto.Cantidad,
                                    PrecioUnitario = producto.Precio,
                                    Descuento = descuentoProducto,
                                    Impuesto = impuestoProducto,
                                    Subtotal = subtotalDetalle
                                }, transaction);

                                // Actualizar el inventario
                                string updateInventarioSql = @"
                                    UPDATE Inventario 
                                    SET Cantidad = Cantidad - @Cantidad 
                                    WHERE ProductoID = @ProductoID AND EmpresaRut = @EmpresaRut";

                                await connection.ExecuteAsync(updateInventarioSql, new
                                {
                                    Cantidad = producto.Cantidad,
                                    ProductoID = productoId,
                                    EmpresaRut = empresaRut
                                }, transaction);
                            }

                            transaction.Commit();
                            return Json(new { success = true, message = "Pedido creado con éxito", ventaId = ventaId, estado = "Pendiente" });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.LogError(ex, "Error al crear el pedido");
                            return Json(new { success = false, message = $"Error al crear el pedido: {ex.Message}" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el pedido");
                return Json(new { success = false, message = $"Error al crear el pedido: {ex.Message}" });
            }
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.VentaID == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Print/5
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.VentaID == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
                        ? Json(new { success = false, message = "Sesión inválida" }) 
                        : RedirectToAction("Login", "Usuarios");
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Eliminar directamente sin cargar los datos
                        await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM DetallesVenta WHERE VentaID = {0}", id);

                        var venta = await _context.Ventas
                            .FirstOrDefaultAsync(v => v.VentaID == id && v.EmpresaRut == empresaRut);

                        if (venta == null)
                        {
                            await transaction.RollbackAsync();
                            return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
                                ? Json(new { success = false, message = "Venta no encontrada" }) 
                                : NotFound();
                        }

                        _context.Ventas.Remove(venta);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
                            ? Json(new { success = true, message = "Venta eliminada correctamente" }) 
                            : RedirectToAction("Index", "ReporteVentas");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la venta {VentaId}", id);
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
                    ? StatusCode(500, new { success = false, message = "Error al eliminar la venta" }) 
                    : RedirectToAction("Index", "ReporteVentas");
            }
        }

        public async Task<IActionResult> ListaVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return View(ventas);
        }

        public async Task<IActionResult> ObtenerDetalles(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.VentaID == id);

            if (venta == null)
            {
                return NotFound();
            }

            return PartialView("_DetallesVenta", venta);
        }

        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            try
            {
                var venta = await _context.Ventas
                    .FirstOrDefaultAsync(v => v.VentaID == id);

                if (venta == null)
                {
                    return Json(new { success = false, message = "Venta no encontrada" });
                }

                // Actualizar solo el estado de la venta
                venta.Estado = "Anulada";
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al anular la venta {id}", ex);
                return Json(new { success = false, message = "Error al anular la venta" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarVenta([FromBody] VentaIdViewModel data)
        {
            try
            {
                var venta = await _context.Ventas
                    .FirstOrDefaultAsync(v => v.VentaID == data.Id);

                if (venta == null)
                {
                    return Json(new { success = false, message = "Venta no encontrada" });
                }

                // Actualizar el estado de la venta, la propina y el método de pago
                venta.Estado = "Completada";
                venta.Propina = data.Propina;
                venta.MetodoPago = data.MedioPago;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al completar la venta", ex);
                return Json(new { success = false, message = "Error al completar la venta" });
            }
        }
    }
} 