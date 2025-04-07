using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentaPOS.Data;
using VentaPOS.Models;
using VentaPOS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using VentaPOS.Helpers;

namespace VentaPOS.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly VentaPosContext _context;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(VentaPosContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var viewModel = new ProductosIndexVM
            {
                Productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => p.EmpresaRut == empresaRut)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync(),
                Categorias = await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Productos/Buscar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buscar([FromBody] FiltroProductoVM filtro)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return Json(new { success = false, message = "Sesión no válida" });
            }

            try
            {
                _logger.LogInformation("Iniciando búsqueda de productos. Filtros: {@Filtro}", filtro);

                // Consulta base
                var query = _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => p.EmpresaRut == empresaRut)
                    .AsNoTracking(); // Mejora el rendimiento para consultas de solo lectura

                // Aplicar filtros
                query = AplicarFiltros(query, filtro);

                // Ejecutar la consulta y transformar resultados
                var productos = await query
                    .OrderBy(p => p.Nombre)
                    .Select(p => new
                    {
                        p.ProductoID,
                        p.Codigo,
                        p.Nombre,
                        p.Descripcion,
                        Precio = $"${p.Precio:N0}",
                        p.Stock,
                        Categoria = p.Categoria.Nombre,
                        UltimaActualizacion = p.UltimaActualizacion.HasValue ?
                            p.UltimaActualizacion.Value.ToString("dd/MM/yyyy HH:mm") : "-"
                    })
                    .ToListAsync();

                _logger.LogInformation("Búsqueda completada. Encontrados {Count} productos", productos.Count);

                return Json(new
                {
                    success = true,
                    data = productos,
                    totalRegistros = productos.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return Json(new { success = false, message = "Error al buscar productos" });
            }
        }

        private static IQueryable<Producto> AplicarFiltros(IQueryable<Producto> query, FiltroProductoVM filtro)
        {
            if (filtro == null) return query;

            // Filtro por categoría
            if (filtro.CategoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaID == filtro.CategoriaId);
            }

            // Filtro por texto (descripción, nombre o código)
            if (!string.IsNullOrWhiteSpace(filtro.Descripcion))
            {
                var searchTerm = filtro.Descripcion.Trim().ToLower();
                query = query.Where(p =>
                    EF.Functions.Like(p.Descripcion.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Nombre.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Codigo.ToLower(), $"%{searchTerm}%"));
            }

            return query;
        }

        // GET: Productos/Create
        public async Task<IActionResult> Create()
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            await PrepararCategorias(empresaRut);
            
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CreateModal", new Producto { EmpresaRut = empresaRut });
            }
            
            return View(new Producto { EmpresaRut = empresaRut });
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Descripcion,Precio,Stock,CategoriaID")] Producto producto)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            try
            {
                ModelState.Remove("Empresa");
                ModelState.Remove("Categoria");
                ModelState.Remove("EmpresaRut");

                if (ModelState.IsValid)
                {
                    // Asignar valores requeridos
                    producto.EmpresaRut = empresaRut;
                    producto.UltimaActualizacion = DateTime.Now;

                    // Verificar que la categoría pertenece a la empresa
                    var categoria = await _context.Categorias
                        .FirstOrDefaultAsync(c => c.CategoriaID == producto.CategoriaID && c.EmpresaRut == empresaRut);

                    if (categoria == null)
                    {
                        ModelState.AddModelError("CategoriaID", "La categoría seleccionada no es válida");
                        await PrepararCategorias(empresaRut);
                        return View(producto);
                    }

                    // Verificar si ya existe un producto con el mismo código
                    var productoExistente = await _context.Productos
                        .AnyAsync(p => p.Codigo == producto.Codigo && p.EmpresaRut == empresaRut);

                    if (productoExistente)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un producto con este código");
                        await PrepararCategorias(empresaRut);
                        return View(producto);
                    }

                    // Asignar la categoría
                    producto.Categoria = categoria;

                    _context.Add(producto);
                    await _context.SaveChangesAsync();

                    if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true });
                    }
                    
                    TempData["Mensaje"] = "Producto creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto: {Message}", ex.Message);
                
                if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    await PrepararCategorias(empresaRut);
                    return Json(new { 
                        success = false, 
                        form = await this.RenderViewAsync("_CreateModal", producto, true) 
                    });
                }
                
                ModelState.AddModelError("", $"Error al crear el producto: {ex.Message}");
            }

            await PrepararCategorias(empresaRut);
            
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { 
                    success = false, 
                    form = await this.RenderViewAsync("_CreateModal", producto, true) 
                });
            }
            
            return View(producto);
        }

        private async Task PrepararCategorias(string empresaRut)
        {
            ViewData["CategoriaID"] = new SelectList(
                await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync(),
                "CategoriaID",
                "Nombre"
            );
        }

        // GET: Productos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.ProductoID == id && m.EmpresaRut == empresaRut);

            if (producto == null)
            {
                return NotFound();
            }

            return Json(new { success = true, producto = producto });
        }

        // POST: Productos/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] Producto producto)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return Json(new { success = false, message = "Sesión no válida" });
            }

            try
            {
                var productoExistente = await _context.Productos
                    .FirstOrDefaultAsync(p => p.ProductoID == producto.ProductoID && p.EmpresaRut == empresaRut);

                if (productoExistente == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado" });
                }

                // Actualizar los campos
                productoExistente.Codigo = producto.Codigo;
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Descripcion = producto.Descripcion;
                productoExistente.Precio = producto.Precio;
                productoExistente.Stock = producto.Stock;
                productoExistente.CategoriaID = producto.CategoriaID;
                productoExistente.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.ProductoID == id && p.EmpresaRut == empresaRut);

            if (producto == null)
            {
                return NotFound();
            }

            try
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Producto eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el producto: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoID == id);
        }
    }
} 