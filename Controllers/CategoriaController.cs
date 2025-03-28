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

                await _context.Categorias.AddRangeAsync(categoriasDefault);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Categorías inicializadas correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al inicializar las categorías: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 