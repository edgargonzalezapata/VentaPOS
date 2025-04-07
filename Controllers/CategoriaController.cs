using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VentaPOS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;
using VentaPOS.Data;

namespace VentaPOS.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly VentaPosContext _context;

        public CategoriaController(VentaPosContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var categorias = _context.Categorias
                .Where(c => c.EmpresaRut == empresaRut)
                .ToList();
            return View(categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromBody] Categoria categoria)
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return Json(new { success = false, message = "Sesión no válida" });
                }

                categoria.EmpresaRut = empresaRut;
                _context.Categorias.Add(categoria);
                _context.SaveChanges();

                return Json(new { success = true, categoria });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] int id)
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                var categoria = _context.Categorias
                    .FirstOrDefault(c => c.CategoriaID == id && c.EmpresaRut == empresaRut);

                if (categoria == null)
                {
                    return Json(new { success = false, message = "Categoría no encontrada" });
                }

                _context.Categorias.Remove(categoria);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            var categoria = _context.Categorias
                .FirstOrDefault(c => c.CategoriaID == id && c.EmpresaRut == empresaRut);

            if (categoria == null)
            {
                return NotFound();
            }

            return Json(new { success = true, categoria });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update([FromBody] Categoria categoria)
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                var categoriaExistente = _context.Categorias
                    .FirstOrDefault(c => c.CategoriaID == categoria.CategoriaID && c.EmpresaRut == empresaRut);

                if (categoriaExistente == null)
                {
                    return Json(new { success = false, message = "Categoría no encontrada" });
                }

                categoriaExistente.Nombre = categoria.Nombre;
                categoriaExistente.Descripcion = categoria.Descripcion;
                _context.SaveChanges();

                return Json(new { success = true, categoria = categoriaExistente });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Categoria/InicializarCategorias
        [HttpPost]
        public async Task<IActionResult> InicializarCategorias()
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return RedirectToAction("Login", "Usuarios");
                }

                // Verificar categorías existentes
                var categoriasExistentes = await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .Select(c => c.Nombre)
                    .ToListAsync();

                var categoriasDefault = new List<Categoria>
                {
                    // Cafetería
                    new Categoria { Nombre = "Café y Bebidas Calientes", Descripcion = "Café, té, chocolate caliente y otras bebidas calientes", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Pastelería", Descripcion = "Pasteles, galletas, muffins y otros productos horneados", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Sándwiches", Descripcion = "Sándwiches fríos y calientes", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Bebidas Frías", Descripcion = "Jugos, refrescos, smoothies y otras bebidas frías", EmpresaRut = empresaRut },
                    
                    // Rotisería
                    new Categoria { Nombre = "Pollos a la Brasa", Descripcion = "Pollos enteros y porciones", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Acompañamientos", Descripcion = "Papas fritas, ensaladas y otros acompañamientos", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Platos Preparados", Descripcion = "Platos del día y especialidades", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Postres", Descripcion = "Postres caseros y dulces", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Salsas", Descripcion = "Salsas y aderezos", EmpresaRut = empresaRut },
                    
                    // Adicionales
                    new Categoria { Nombre = "Promociones", Descripcion = "Combos y ofertas especiales", EmpresaRut = empresaRut },
                    new Categoria { Nombre = "Snacks", Descripcion = "Bocadillos y aperitivos", EmpresaRut = empresaRut }
                };

                // Filtrar solo las categorías que no existen
                var nuevasCategorias = categoriasDefault
                    .Where(c => !categoriasExistentes.Contains(c.Nombre))
                    .ToList();

                if (nuevasCategorias.Any())
                {
                    await _context.Categorias.AddRangeAsync(nuevasCategorias);
                    await _context.SaveChangesAsync();
                }

                // Obtener todas las categorías (existentes y nuevas) para sus IDs
                var categorias = await _context.Categorias
                    .Where(c => c.EmpresaRut == empresaRut)
                    .ToDictionaryAsync(c => c.Nombre, c => c.CategoriaID);

                // Verificar productos existentes por código
                var productosExistentes = await _context.Productos
                    .Where(p => p.EmpresaRut == empresaRut)
                    .Select(p => p.Codigo)
                    .ToListAsync();

                // Lista de productos predeterminados
                var productos = new List<Producto>();

                // Café y Bebidas Calientes
                if (categorias.TryGetValue("Café y Bebidas Calientes", out int cafeId))
                {
                    productos.AddRange(new[]
                    {
                        new Producto { Codigo = "CAF001", Nombre = "Café Americano", Descripcion = "Café negro tradicional", Precio = 1500, Stock = 100, CategoriaID = cafeId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "CAF002", Nombre = "Café Expreso", Descripcion = "Shot de café concentrado", Precio = 1200, Stock = 100, CategoriaID = cafeId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "CAF003", Nombre = "Capuchino", Descripcion = "Café con leche espumada", Precio = 2000, Stock = 100, CategoriaID = cafeId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "CAF004", Nombre = "Té", Descripcion = "Variedad de tés", Precio = 1000, Stock = 100, CategoriaID = cafeId, EmpresaRut = empresaRut }
                    });
                }

                // Pastelería
                if (categorias.TryGetValue("Pastelería", out int pasteleriaId))
                {
                    productos.AddRange(new[]
                    {
                        new Producto { Codigo = "PAS001", Nombre = "Torta de Chocolate", Descripcion = "Porción individual", Precio = 2500, Stock = 50, CategoriaID = pasteleriaId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "PAS002", Nombre = "Croissant", Descripcion = "Croissant de mantequilla", Precio = 1500, Stock = 50, CategoriaID = pasteleriaId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "PAS003", Nombre = "Muffin", Descripcion = "Varios sabores", Precio = 1200, Stock = 50, CategoriaID = pasteleriaId, EmpresaRut = empresaRut }
                    });
                }

                // Sándwiches
                if (categorias.TryGetValue("Sándwiches", out int sandwichId))
                {
                    productos.AddRange(new[]
                    {
                        new Producto { Codigo = "SAN001", Nombre = "Sándwich de Pollo", Descripcion = "Con lechuga y tomate", Precio = 3500, Stock = 30, CategoriaID = sandwichId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "SAN002", Nombre = "Sándwich Vegetariano", Descripcion = "Con palta y verduras", Precio = 3000, Stock = 30, CategoriaID = sandwichId, EmpresaRut = empresaRut }
                    });
                }

                // Pollos a la Brasa
                if (categorias.TryGetValue("Pollos a la Brasa", out int polloId))
                {
                    productos.AddRange(new[]
                    {
                        new Producto { Codigo = "POL001", Nombre = "1/4 Pollo", Descripcion = "Con papas fritas y ensalada", Precio = 4500, Stock = 50, CategoriaID = polloId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "POL002", Nombre = "1/2 Pollo", Descripcion = "Con papas fritas y ensalada", Precio = 8500, Stock = 50, CategoriaID = polloId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "POL003", Nombre = "Pollo Entero", Descripcion = "Con papas fritas y ensalada", Precio = 15000, Stock = 30, CategoriaID = polloId, EmpresaRut = empresaRut }
                    });
                }

                // Acompañamientos
                if (categorias.TryGetValue("Acompañamientos", out int acompId))
                {
                    productos.AddRange(new[]
                    {
                        new Producto { Codigo = "ACO001", Nombre = "Papas Fritas", Descripcion = "Porción regular", Precio = 2500, Stock = 100, CategoriaID = acompId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "ACO002", Nombre = "Ensalada Mixta", Descripcion = "Lechuga, tomate, zanahoria", Precio = 2000, Stock = 50, CategoriaID = acompId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "ACO003", Nombre = "Arroz", Descripcion = "Porción de arroz graneado", Precio = 1500, Stock = 100, CategoriaID = acompId, EmpresaRut = empresaRut }
                    });
                }

                // Bebidas Frías
                if (categorias.TryGetValue("Bebidas Frías", out int bebidasId))
                {
                    productos.AddRange(new[]
                    {
                        new Producto { Codigo = "BEB001", Nombre = "Coca-Cola", Descripcion = "350ml", Precio = 1500, Stock = 100, CategoriaID = bebidasId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "BEB002", Nombre = "Jugo Natural", Descripcion = "Varios sabores", Precio = 2000, Stock = 50, CategoriaID = bebidasId, EmpresaRut = empresaRut },
                        new Producto { Codigo = "BEB003", Nombre = "Agua Mineral", Descripcion = "500ml", Precio = 1000, Stock = 100, CategoriaID = bebidasId, EmpresaRut = empresaRut }
                    });
                }

                // Filtrar solo los productos que no existen
                var nuevosProductos = productos
                    .Where(p => !productosExistentes.Contains(p.Codigo))
                    .ToList();

                if (nuevosProductos.Any())
                {
                    await _context.Productos.AddRangeAsync(nuevosProductos);
                    await _context.SaveChangesAsync();
                }

                TempData["Mensaje"] = "Categorías y productos inicializados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al inicializar las categorías y productos: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 