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
        public async Task<IActionResult> Index(string searchTerm, int? categoriaID, decimal? precioMin, 
            decimal? precioMax, string stockFilter, string ordenarPor)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            
            // Guardar filtros en ViewBag para mantenerlos en la vista
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoriaID = categoriaID;
            ViewBag.PrecioMin = precioMin;
            ViewBag.PrecioMax = precioMax;
            ViewBag.StockFilter = stockFilter;
            ViewBag.OrdenarPor = ordenarPor;
            
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.EmpresaRut == empresaRut);

            // Filtro por texto (nombre o código)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => 
                    EF.Functions.Like(p.Nombre, $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Codigo, $"%{searchTerm}%") ||
                    (p.Descripcion != null && EF.Functions.Like(p.Descripcion, $"%{searchTerm}%"))
                );
            }
            
            // Filtro por categoría
            if (categoriaID.HasValue)
            {
                query = query.Where(p => p.CategoriaID == categoriaID.Value);
            }
            
            // Filtro por rango de precio
            if (precioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= precioMin.Value);
            }
            
            if (precioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= precioMax.Value);
            }
            
            // Filtro por stock
            switch (stockFilter)
            {
                case "disponible":
                    query = query.Where(p => p.Stock > 0);
                    break;
                case "agotado":
                    query = query.Where(p => p.Stock <= 0);
                    break;
                case "bajo":
                    query = query.Where(p => p.Stock > 0 && p.Stock < 10);
                    break;
            }
            
            // Ordenamiento
            query = ordenarPor switch
            {
                "nombre" => query.OrderBy(p => p.Nombre),
                "nombre_desc" => query.OrderByDescending(p => p.Nombre),
                "precio_asc" => query.OrderBy(p => p.Precio),
                "precio_desc" => query.OrderByDescending(p => p.Precio),
                "stock_asc" => query.OrderBy(p => p.Stock),
                "stock_desc" => query.OrderByDescending(p => p.Stock),
                "categoria" => query.OrderBy(p => p.Categoria.Nombre),
                _ => query.OrderBy(p => p.Nombre)
            };

            var model = new ProductosIndexVM
            {
                Productos = await query.ToListAsync(),
                Categorias = await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync()
            };

            return View(model);
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

            ViewData["CategoriaID"] = new SelectList(
                await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync(), 
                "CategoriaID", 
                "Nombre", 
                producto.CategoriaID
            );

            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoID,Codigo,Nombre,Descripcion,Precio,Stock,CategoriaID,EmpresaRut")] Producto producto)
        {
            if (id != producto.ProductoID)
            {
                return NotFound();
            }

            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            producto.EmpresaRut = empresaRut;
            producto.UltimaActualizacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Producto actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["CategoriaID"] = new SelectList(
                await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync(), 
                "CategoriaID", 
                "Nombre", 
                producto.CategoriaID
            );

            return View(producto);
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