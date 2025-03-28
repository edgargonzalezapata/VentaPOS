using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentaPOS.Data;
using VentaPOS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace VentaPOS.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly VentaPosContext _context;
        private readonly ILogger<StockController> _logger;

        public StockController(VentaPosContext context, ILogger<StockController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Stock
        public async Task<IActionResult> Index(string searchTerm, int? categoriaID, string stockFilter, string ordenarPor)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            
            // Guardar filtros en ViewBag para mantenerlos en la vista
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoriaID = categoriaID;
            ViewBag.StockFilter = stockFilter;
            ViewBag.OrdenarPor = ordenarPor;
            
            // Obtener las categorías para el filtro
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.EmpresaRut == empresaRut)
                .Select(c => new { c.CategoriaID, c.Nombre })
                .ToListAsync();
            
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
            
            // Ordenar resultados
            query = ordenarPor switch
            {
                "nombre_asc" => query.OrderBy(p => p.Nombre),
                "nombre_desc" => query.OrderByDescending(p => p.Nombre),
                "codigo_asc" => query.OrderBy(p => p.Codigo),
                "codigo_desc" => query.OrderByDescending(p => p.Codigo),
                "precio_asc" => query.OrderBy(p => p.Precio),
                "precio_desc" => query.OrderByDescending(p => p.Precio),
                "stock_asc" => query.OrderBy(p => p.Stock),
                "stock_desc" => query.OrderByDescending(p => p.Stock),
                _ => query.OrderBy(p => p.Nombre)
            };
            
            var productos = await query.ToListAsync();
            
            // Calcular resumen de stock
            ViewBag.TotalProductos = productos.Count;
            ViewBag.ProductosDisponibles = productos.Count(p => p.Stock > 0);
            ViewBag.ProductosAgotados = productos.Count(p => p.Stock <= 0);
            ViewBag.ProductosBajoStock = productos.Count(p => p.Stock > 0 && p.Stock < 10);
            
            return View(productos);
        }

        // POST: Stock/AjustarStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjustarStock(int productoId, int cantidad, string tipo)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            
            if (producto == null)
            {
                return NotFound();
            }
            
            try
            {
                // Actualizar stock según el tipo de ajuste
                if (tipo == "sumar")
                {
                    producto.Stock += cantidad;
                }
                else if (tipo == "restar")
                {
                    if (producto.Stock - cantidad < 0)
                    {
                        producto.Stock = 0; // Evitar stock negativo
                    }
                    else
                    {
                        producto.Stock -= cantidad;
                    }
                }
                else if (tipo == "establecer")
                {
                    producto.Stock = cantidad >= 0 ? cantidad : 0;
                }
                
                await _context.SaveChangesAsync();
                return Json(new { success = true, nuevoStock = producto.Stock });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ajustar el stock del producto {ProductoId}", productoId);
                return Json(new { success = false, message = "Error al ajustar el stock" });
            }
        }
    }
} 